using CommunityToolkit.Mvvm.ComponentModel;

namespace Bitwarden_Autofill.ViewModel;

internal partial class UnlockViewModel : ObservableObject
{
    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string? errorMessage;
}
