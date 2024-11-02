using CommunityToolkit.Mvvm.ComponentModel;

namespace Bitwarden_Autofill.ViewModel;

internal partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string version = string.Empty;

    [ObservableProperty]
    private string edition = string.Empty;
}
