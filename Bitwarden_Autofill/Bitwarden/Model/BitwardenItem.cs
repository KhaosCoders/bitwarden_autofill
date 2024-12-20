namespace Bitwarden_Autofill.Bitwarden.Model;

public class BitwardenItem
{
    public string? Id { get; set; }
    public EItemType Type { get; set; }
    public string? Name { get; set; }
    public bool Favorite { get; set; }

    public BitwardenLogin? Login { get; set; }
}
