using Bitwarden_Autofill.ViewModel;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bitwarden_Autofill.View;

/// <summary>
/// The main window of the application. This window hosts all sorts of pages.
/// </summary>
internal sealed partial class MainWindow : Window
{
    private const int WindowWidth = 600;
    private const int WindowHeight = 800;

    public MainWindowViewModel ViewModel { get; set; }

    public Page? CurrentPage => ContentFrame.Content as Page;

    public MainWindow()
    {
        AppWindow.MoveAndResize(new(
            (AppWindow.ClientSize.Width - WindowWidth) / 2,
            (AppWindow.ClientSize.Height - WindowHeight) / 2,
            WindowWidth,
            WindowHeight));

        this.InitializeComponent();
        ViewModel = new()
        {
            Version = $"v{(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0")}",
            Edition = "Free (beta)"
        };

        this.Closed += MainWindow_Closed;
    }

    private void MainWindow_Closed(object sender, WindowEventArgs e)
    {
        e.Handled = true;
        this.Hide();
    }

    public void ShowPage(Page page)
    {
        ContentFrame.Content = page;
    }

    public void Move(int x, int y)
    {
        AppWindow.Move(new(x, y));
    }
}
