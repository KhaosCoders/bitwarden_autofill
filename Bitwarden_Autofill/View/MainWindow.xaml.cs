using Bitwarden_Autofill.ViewModel;
using H.NotifyIcon;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Rect = Windows.Foundation.Rect;

namespace Bitwarden_Autofill.View;

/// <summary>
/// The main window of the application. This window hosts all sorts of pages.
/// </summary>
internal sealed partial class MainWindow : Window
{
    private const int WindowWidth = 800;
    private const int WindowHeight = 800;
    private readonly double minSearchWidth;
    private readonly GridLength searchWidth;

    public MainWindowViewModel ViewModel { get; set; }

    public Page? CurrentPage => ContentFrame.Content as Page;

    public bool HideSearch
    {
        set
        {
            SearchColumn.MinWidth = value ? 0 : minSearchWidth;
            SearchColumn.Width = value ? new GridLength(0) : searchWidth;
        }
    }

    private readonly AppWindow appWindow;

    public MainWindow()
    {
        appWindow = AppWindow;

        // Disable the maximize button
        appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
        var presenter = appWindow.Presenter as OverlappedPresenter;
        if (presenter != null)
        {
            presenter.IsMaximizable = false;
        }

        CenterWindowOnScreen();

        ViewModel = new()
        {
            Version = $"v{(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0")}",
            Edition = "Free (beta)"
        };
        InitializeComponent();

        Closed += MainWindow_Closed;
        appWindow.Changed += AppWindow_Changed;
        Activated += MainWindow_Activated;
        AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
        AppTitleBar.Loaded += AppTitleBar_Loaded;

        // Try to extend the content into the title bar
        ExtendsContentIntoTitleBar = true;
        if (ExtendsContentIntoTitleBar == true)
        {
            appWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        }
        TitleBarTextBlock.Text = "Autofill";

        // Hide the search box by default
        minSearchWidth = SearchColumn.MinWidth;
        searchWidth = SearchColumn.Width;
        HideSearch = true;
    }

    private void CenterWindowOnScreen()
    {
        appWindow.MoveAndResize(new(
            (appWindow.ClientSize.Width - WindowWidth) / 2,
            (appWindow.ClientSize.Height - WindowHeight) / 2,
            WindowWidth,
            WindowHeight));
    }

    public void ShowPage(Page page)
    {
        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Page page)
            {
                page.Loaded -= Page_Loaded;
                page.Focus(FocusState.Keyboard);
            }
        }

        HideSearch = page is not IPageWithSearch;
        page.Loaded += Page_Loaded;
        ContentFrame.Content = page;
    }

    public void Move(int x, int y)
    {
        AppWindow.Move(new(x, y));
    }

    private void MainWindow_Closed(object sender, WindowEventArgs e)
    {
        e.Handled = true;
        this.Hide();
    }

    private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
    {
        if (ExtendsContentIntoTitleBar == true)
        {
            // Set the initial interactive regions.
            SetRegionsForCustomTitleBar();
        }
    }

    private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (ExtendsContentIntoTitleBar == true)
        {
            // Update interactive regions if the size of the window changes.
            SetRegionsForCustomTitleBar();
        }
    }

    private void SetRegionsForCustomTitleBar()
    {
        // Specify the interactive regions of the title bar.

        double scaleAdjustment = AppTitleBar.XamlRoot.RasterizationScale;

        RightPaddingColumn.Width = new GridLength(appWindow.TitleBar.RightInset / scaleAdjustment);
        LeftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);

        // Get the rectangle around the AutoSuggestBox control.
        GeneralTransform transform = TitleBarSearchBox.TransformToVisual(null);
        Rect bounds = transform.TransformBounds(new Rect(0, 0,
                                                         TitleBarSearchBox.ActualWidth,
                                                         TitleBarSearchBox.ActualHeight));
        Windows.Graphics.RectInt32 SearchBoxRect = GetRect(bounds, scaleAdjustment);

        // Get the rectangle around the PersonPicture control.
        transform = PersonPic.TransformToVisual(null);
        bounds = transform.TransformBounds(new Rect(0, 0,
                                                    PersonPic.ActualWidth,
                                                    PersonPic.ActualHeight));
        Windows.Graphics.RectInt32 PersonPicRect = GetRect(bounds, scaleAdjustment);

        var rectArray = new Windows.Graphics.RectInt32[] { SearchBoxRect, PersonPicRect };

        InputNonClientPointerSource nonClientInputSrc =
            InputNonClientPointerSource.GetForWindowId(AppWindow.Id);
        nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
    }

    private Windows.Graphics.RectInt32 GetRect(Rect bounds, double scale)
    {
        return new Windows.Graphics.RectInt32(
            _X: (int)Math.Round(bounds.X * scale),
            _Y: (int)Math.Round(bounds.Y * scale),
            _Width: (int)Math.Round(bounds.Width * scale),
            _Height: (int)Math.Round(bounds.Height * scale)
        );
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
        {
            TitleBarTextBlock.Foreground =
                (SolidColorBrush)Application.Current.Resources["WindowCaptionForegroundDisabled"];
        }
        else
        {
            TitleBarTextBlock.Foreground =
                (SolidColorBrush)Application.Current.Resources["WindowCaptionForeground"];
        }
    }

    private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidPresenterChange)
        {
            switch (sender.Presenter.Kind)
            {
                case AppWindowPresenterKind.CompactOverlay:
                    // Compact overlay - hide custom title bar
                    // and use the default system title bar instead.
                    AppTitleBar.Visibility = Visibility.Collapsed;
                    sender.TitleBar.ResetToDefault();
                    break;

                case AppWindowPresenterKind.FullScreen:
                    // Full screen - hide the custom title bar
                    // and the default system title bar.
                    AppTitleBar.Visibility = Visibility.Collapsed;
                    sender.TitleBar.ExtendsContentIntoTitleBar = true;
                    break;

                case AppWindowPresenterKind.Overlapped:
                    // Normal - hide the system title bar
                    // and use the custom title bar instead.
                    AppTitleBar.Visibility = Visibility.Visible;
                    sender.TitleBar.ExtendsContentIntoTitleBar = true;
                    break;

                default:
                    // Use the default system title bar.
                    sender.TitleBar.ResetToDefault();
                    break;
            }
        }
    }

    private void TitleBarSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) =>
        (CurrentPage as IPageWithSearch)?.SearchTextChanged(sender.Text);
}
