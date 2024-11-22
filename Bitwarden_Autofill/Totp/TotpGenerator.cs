using OtpNet;
using System;

namespace Bitwarden_Autofill.Totp;

/// <summary>
/// Provides methods to generate TOTP (Time-based One-Time Password) from a URI or a secret.
/// </summary>
internal static class TotpGenerator
{
    /// <summary>
    /// Generates a TOTP (Time-based One-Time Password) from a given URI.
    /// </summary>
    /// <param name="url">The URI containing the TOTP parameters.</param>
    /// <returns>The generated TOTP as a string, or null if the URI is invalid or missing required parameters.</returns>
    public static string? FromUri(string url)
    {
        Uri uri = new(url);
        var parameters = System.Web.HttpUtility.ParseQueryString(uri.Query);
        if (parameters == null) return default;

        string? secret = parameters["secret"];
        if (secret == null) return default;

        string? algorithm = parameters["algorithm"];
        string? digits = parameters["digits"];
        string? period = parameters["period"];

        return FromSecret(secret, algorithm, digits, period);
    }

    /// <summary>
    /// Generates a TOTP (Time-based One-Time Password) from a given secret.
    /// </summary>
    /// <param name="secret">The secret key used to generate the TOTP.</param>
    /// <param name="algorithm">The hashing algorithm to use (e.g., SHA1, SHA256, SHA512). Defaults to SHA1 if not provided.</param>
    /// <param name="digits">The number of digits in the generated TOTP. Defaults to 6 if not provided.</param>
    /// <param name="period">The time period in seconds for which the TOTP is valid. Defaults to 30 seconds if not provided.</param>
    /// <returns>The generated TOTP as a string.</returns>
    public static string FromSecret(string secret, string? algorithm, string? digits, string? period)
    {
        var bytes = Base32Encoding.ToBytes(secret);
        int totpSize = int.TryParse(digits, out int result1) ? result1 : 6;
        int step = int.TryParse(period, out int result2) ? result2 : 30;
        OtpHashMode mode = algorithm switch
        {
            "SHA256" => OtpHashMode.Sha256,
            "SHA512" => OtpHashMode.Sha512,
            _ => OtpHashMode.Sha1,
        };

        OtpNet.Totp totp = new(bytes, step, mode, totpSize);
        return totp.ComputeTotp();
    }
}
