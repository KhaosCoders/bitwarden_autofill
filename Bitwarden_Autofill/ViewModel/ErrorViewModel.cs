using CommunityToolkit.Mvvm.ComponentModel;

namespace Bitwarden_Autofill.ViewModel;

internal partial class ErrorViewModel : ObservableObject
{
    [ObservableProperty]
    private string message = "Error occured";
}
