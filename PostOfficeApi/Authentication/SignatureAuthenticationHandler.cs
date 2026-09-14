using System.Globalization;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace PostOfficeApi.Authentication;

public class SignatureAuthenticationOptions : AuthenticationSchemeOptions
{
}

/// <summary>
/// Authenticates data providers with a per-request HMAC signature.
/// Required headers: X-App-Number, X-User-Token, X-Call-DateTime, X-Signature.
/// </summary>
public class SignatureAuthenticationHandler : AuthenticationHandler<SignatureAuthenticationOptions>
{
    private readonly ApiClientOptions _clientOptions;
    private readonly IMemoryCache _replayCache;

    public SignatureAuthenticationHandler(
        IOptionsMonitor<SignatureAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptions<ApiClientOptions> clientOptions,
        IMemoryCache replayCache)
        : base(options, logger, encoder)
    {
        _clientOptions = clientOptions.Value;
        _replayCache = replayCache;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!TryGetHeader(SignatureAuthenticationDefaults.AppNumberHeader, out var appNumber) ||
            !TryGetHeader(SignatureAuthenticationDefaults.UserTokenHeader, out var userToken) ||
            !TryGetHeader(SignatureAuthenticationDefaults.CallDateTimeHeader, out var callDateTime) ||
            !TryGetHeader(SignatureAuthenticationDefaults.SignatureHeader, out var signature))
        {
            return AuthenticateResult.Fail(
                $"Missing one of the required headers: {SignatureAuthenticationDefaults.AppNumberHeader}, " +
                $"{SignatureAuthenticationDefaults.UserTokenHeader}, " +
                $"{SignatureAuthenticationDefaults.CallDateTimeHeader}, " +
                $"{SignatureAuthenticationDefaults.SignatureHeader}.");
        }

        if (!DateTimeOffset.TryParse(callDateTime, CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out var callTimestamp))
        {
            return AuthenticateResult.Fail($"{SignatureAuthenticationDefaults.CallDateTimeHeader} is not a valid ISO 8601 datetime.");
        }

        var drift = DateTimeOffset.UtcNow - callTimestamp.ToUniversalTime();
        if (drift.Duration() > _clientOptions.AllowedClockSkew)
        {
            return AuthenticateResult.Fail(
                $"{SignatureAuthenticationDefaults.CallDateTimeHeader} is outside the allowed window of " +
                $"{_clientOptions.AllowedClockSkew.TotalMinutes:0} minute(s).");
        }

        var client = _clientOptions.Clients
            .FirstOrDefault(c => string.Equals(c.AppNumber, appNumber, StringComparison.Ordinal));

        if (client is null)
        {
            return AuthenticateResult.Fail("Unknown app number.");
        }

        if (client.UserTokens.Count > 0 && !client.UserTokens.Contains(userToken, StringComparer.Ordinal))
        {
            return AuthenticateResult.Fail("Unknown user token for this app number.");
        }

        var body = await ReadBodyAsync();
        var stringToSign = RequestSignature.BuildStringToSign(
            appNumber,
            userToken,
            callDateTime,
            Request.Method,
            Request.Path + Request.QueryString,
            body);

        var expectedSignature = RequestSignature.Compute(client.AppSecret, stringToSign);
        if (!RequestSignature.Matches(expectedSignature, signature))
        {
            Logger.LogWarning("Invalid signature for app number {AppNumber}", appNumber);
            return AuthenticateResult.Fail("Invalid signature.");
        }

        var replayKey = $"{appNumber}:{signature}";
        if (!_replayCache.TryGetValue(replayKey, out _))
        {
            _replayCache.Set(replayKey, true, _clientOptions.AllowedClockSkew * 2);
        }
        else
        {
            return AuthenticateResult.Fail("Duplicate request signature (replay detected).");
        }

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, appNumber),
                new Claim(ClaimTypes.Name, client.DisplayName ?? appNumber),
                new Claim("user_token", userToken)
            },
            Scheme.Name);

        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }

    private bool TryGetHeader(string name, out string value)
    {
        value = Request.Headers[name].ToString();
        return !string.IsNullOrWhiteSpace(value);
    }

    private async Task<string> ReadBodyAsync()
    {
        Request.EnableBuffering();
        Request.Body.Position = 0;

        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        Request.Body.Position = 0;

        return body;
    }
}
