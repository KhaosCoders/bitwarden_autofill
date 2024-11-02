using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Bitwarden_Autofill.ViewModel;

internal partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string clientId = string.Empty;

    [ObservableProperty]
    private string clientSecret = string.Empty;

    [ObservableProperty]
    private string validationError = string.Empty;

    [ObservableProperty]
    private int selectedServerIndex;

    public ObservableCollection<string> Servers { get; set; } =
        [
            "bitwarden.com",
            "bitwarden.eu",
            "custom server"
        ];
}
