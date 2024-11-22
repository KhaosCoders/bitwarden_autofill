using CommunityToolkit.Mvvm.ComponentModel;

namespace Bitwarden_Autofill;

internal partial class DownloadViewModel : ObservableObject
{
    [ObservableProperty]
    private double downloadProgress;

    [ObservableProperty]
    private bool isIndetermined = true;
}
