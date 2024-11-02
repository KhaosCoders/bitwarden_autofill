using Bitwarden_Autofill.View;
using H.NotifyIcon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Drawing;
using System.Threading;

namespace Bitwarden_Autofill.Flow;

internal class UIDispatcher(DispatcherQueue dispatcher, IServiceProvider services)
{
    private readonly DispatcherQueue _dispatcherQueue = dispatcher;
    private readonly IServiceProvider _services = services;
    private MainWindow? _mainWindow;
    private LoadingPage? _loadingPage;

    public nint MainWindowHandle => WinRT.Interop.WindowNative.GetWindowHandle(_mainWindow);

    public double MainWindowWidth => _mainWindow?.AppWindow.ClientSize.Width ?? 0;
    public double MainWindowHeight => _mainWindow?.AppWindow.ClientSize.Height ?? 0;

    public void Dispatch(Action a)
    {
        DispatcherQueueHandler action = () =>
        {
            try
            {
                a();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Unhandled Exception on dispatcher");
                throw;
            }
        };

        if (Environment.CurrentManagedThreadId == 1)
        {
            action.Invoke();
        }
        else
        {
            _dispatcherQueue.TryEnqueue(action);
        }

    }

    public void IndicateLoading()
    {
        Log.Debug("Display loading page");
        Dispatch(() => _mainWindow?.ShowPage(_loadingPage ??= new LoadingPage()));
    }

    public void ShowError(string message)
    {
        if (_mainWindow == null)
        {
            OpenMainWindow();
        }
        ShowPage<ErrorPage>(page => page.ViewModel.Message = message);
    }

    public void ShowPage(Page page)
    {
        Log.Debug("Display page instance: {PageName}", page.GetType().Name);
        Dispatch(() => _mainWindow?.ShowPage(page));
    }

    public T ShowPage<T>(Action<T>? updatePage = default) where T : Page
    {
        Log.Debug("Display page: {PageName}", typeof(T).Name);
        ManualResetEventSlim mre = new();
        T page = default!;
        Dispatch(() =>
        {
            page = _mainWindow?.CurrentPage as T ?? GetService<T>();

            updatePage?.Invoke(page);

            _mainWindow?.ShowPage(page);

            mre.Set();
        });
        mre.Wait();
        return page;
    }

    public T OpenWindow<T>() where T : Window
    {
        Log.Debug("Open window: {WindowName}", typeof(T).Name);
        T window = default!;
        ManualResetEventSlim mre = new();
        Dispatch(() =>
        {
            window = GetService<T>();
            window.Activate();
            mre.Set();
        });
        mre.Wait();
        return window;
    }

    public void CloseWindow(Window window)
    {
        Log.Debug("Close window: {WindowName}", window.GetType().Name);
        Dispatch(window.Close);
    }

    public void OpenMainWindow()
    {
        _mainWindow ??= OpenWindow<MainWindow>();

        if (_mainWindow?.Visible == false)
        {
            _mainWindow.Show();
        }

        // Place main window on top
        Win.Win32Api.SetForegroundWindow(MainWindowHandle);
    }

    public void HideMainWindow()
    {
        if (_mainWindow?.Visible == true)
        {
            _mainWindow.Hide();
        }
    }

    private T GetService<T>() where T : class => _services.GetRequiredService<T>();

    internal void MoveMainWindow(Point focusPoint)
    {
        _mainWindow?.Move(focusPoint.X, focusPoint.Y);
    }
}
