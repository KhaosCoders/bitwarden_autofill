using Microsoft.UI.Xaml;

namespace Bitwarden_Autofill.View;

/// <summary>
/// A window that displays the download progress of the CLI.
/// </summary>
internal sealed partial class DownloadCliWindow : Window
{
    private const int WindowWidth = 700;
    private const int WindowHeight = 140;

    public DownloadCliWindow()
    {
        InitializeComponent();
        AppWindow.MoveAndResize(new(
            (AppWindow.ClientSize.Width - WindowWidth) / 2,
            (AppWindow.ClientSize.Height - WindowHeight) / 2,
            WindowWidth,
            WindowHeight));
        ViewModel = new();
    }

    public DownloadViewModel ViewModel { get; set; }
}
