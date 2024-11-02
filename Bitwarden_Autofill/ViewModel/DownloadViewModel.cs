using CommunityToolkit.Mvvm.ComponentModel;

namespace Bitwarden_Autofill;

internal partial class DownloadViewModel : ObservableObject
{
    public double DownloadProgress
    {
        get => _downloadProgress;
        set => SetProperty(ref _downloadProgress, value);
    }
    private double _downloadProgress;

    public bool IsIndetermined
    {
        get => _indetermined;
        set => SetProperty(ref _indetermined, value);
    }
    private bool _indetermined = true;
}
