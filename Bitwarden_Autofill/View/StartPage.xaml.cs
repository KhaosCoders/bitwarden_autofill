using Bitwarden_Autofill.Flow;
using Bitwarden_Autofill.Options;
using Bitwarden_Autofill.ViewModel;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bitwarden_Autofill.View;

/// <summary>
/// A page that displays a start screen for the application.
/// </summary>
internal sealed partial class StartPage : Page
{
    public StartPage(UIDispatcher dispatcher, IOptionsSnapshot<AppSettings> options, ISettingsStore settingsStore)
    {
        _settingsStore = settingsStore;
        _dispatcher = dispatcher;

        var settings = options.Value;
        ViewModel = new(dispatcher)
        {
            GlobalHotkey = settings.GlobalHotkey!
        };
        InitializeComponent();
    }

    private readonly ISettingsStore _settingsStore;
    private readonly UIDispatcher _dispatcher;

    public StartPageViewModel ViewModel { get; set; }

    private void FirstButton_Click(object sender, RoutedEventArgs e)
    {
        Gallery.SelectedIndex = 1;
        HotkeySelector.Focus(FocusState.Programmatic);
    }

    private void SecondButton_Click(object sender, RoutedEventArgs e)
    {
        Gallery.SelectedIndex = 2;
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        _settingsStore.StoreGlobalHotkey(ViewModel.GlobalHotkey);
        _dispatcher.CloseMainWindow();
    }
}
