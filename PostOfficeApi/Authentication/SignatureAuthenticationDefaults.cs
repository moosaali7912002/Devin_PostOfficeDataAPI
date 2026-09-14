namespace PostOfficeApi.Authentication;

public static class SignatureAuthenticationDefaults
{
    public const string AuthenticationScheme = "SignatureAuth";

    public const string AppNumberHeader = "X-App-Number";
    public const string UserTokenHeader = "X-User-Token";
    public const string CallDateTimeHeader = "X-Call-DateTime";
    public const string SignatureHeader = "X-Signature";
}
