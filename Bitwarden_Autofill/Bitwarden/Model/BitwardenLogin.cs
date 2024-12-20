using System.Collections.Generic;

namespace Bitwarden_Autofill.Bitwarden.Model;

public class BitwardenLogin
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Totp { get; set; }

    public List<ItemUri>? Uris { get; set; }
}
