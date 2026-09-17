using System.Security.Cryptography;
using System.Text;

namespace PostOfficeApi.Authentication;

/// <summary>
/// Builds the canonical string that callers must sign and the resulting HMAC-SHA256 signature.
/// Shared by the server and by any client implementation.
/// </summary>
public static class RequestSignature
{
    public static string BuildStringToSign(
        string appNumber,
        string userToken,
        string callDateTime,
        string httpMethod,
        string pathAndQuery,
        string body)
    {
        var bodyHash = HashBody(body);

        return string.Join('\n',
            appNumber,
            userToken,
            callDateTime,
            httpMethod.ToUpperInvariant(),
            pathAndQuery,
            bodyHash);
    }

    public static string HashBody(string body)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(body ?? string.Empty));
        return Convert.ToBase64String(hash);
    }

    public static string Compute(string appSecret, string stringToSign)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
        var signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
        return Convert.ToBase64String(signature);
    }

    public static bool Matches(string expected, string provided)
    {
        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var providedBytes = Encoding.UTF8.GetBytes(provided);

        return expectedBytes.Length == providedBytes.Length
               && CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }
}
