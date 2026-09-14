namespace PostOfficeApi.Authentication;

public class ApiClientOptions
{
    public const string SectionName = "ApiClients";

    /// <summary>Registered data providers allowed to call the API.</summary>
    public List<ApiClient> Clients { get; set; } = new();

    /// <summary>How far the caller's call datetime may differ from server UTC time.</summary>
    public TimeSpan AllowedClockSkew { get; set; } = TimeSpan.FromMinutes(5);
}

public class ApiClient
{
    /// <summary>Public identifier of the calling application (app_number).</summary>
    public string AppNumber { get; set; } = null!;

    /// <summary>Shared secret used to sign requests (app_secret). Never sent over the wire.</summary>
    public string AppSecret { get; set; } = null!;

    /// <summary>Tokens the application may present as user_token.</summary>
    public List<string> UserTokens { get; set; } = new();

    /// <summary>Optional display name used for logging.</summary>
    public string? DisplayName { get; set; }
}
