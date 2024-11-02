namespace Bitwarden_Autofill.API.Models;

public class BitwardenItem
{
    public EItemType Type { get; set; }
    public string? Name { get; set; }
    public bool Favorite { get; set; }

    public BitwardenLogin? Login { get; set; }
}
