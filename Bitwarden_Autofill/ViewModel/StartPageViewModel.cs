using Bitwarden_Autofill.Flow;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Foundation;

namespace Bitwarden_Autofill.ViewModel;

internal partial class StartPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string globalHotkey = string.Empty;

    [ObservableProperty]
    private bool isAutorunEnabled;

    [ObservableProperty]
    private string autorunError = string.Empty;

    private readonly UIDispatcher _dispatcher;

    public StartPageViewModel(UIDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        PropertyChanged += StartPageViewModel_PropertyChanged;
        Task.Run(TryEnableAutoRun).LogErrors();
    }

    private void TryEnableAutoRun()
    {
        var task = StartupTask.GetAsync("Bitwarden_Autofill");
        while (task.Status == AsyncStatus.Started)
        {
            Thread.Sleep(100);
        }
        StartupTask startupTask = task.GetResults();
        switch (startupTask.State)
        {
            case StartupTaskState.Disabled:
                EnableAutoRun(startupTask);
                break;
            case StartupTaskState.DisabledByUser:
                _dispatcher.Dispatch(() =>
                {
                    AutorunError = "Autorun can only be activated by the user.";
                    IsAutorunEnabled = false;
                });
                break;
            case StartupTaskState.DisabledByPolicy:
                _dispatcher.Dispatch(() =>
                {
                    AutorunError = "Autorun is disabled by administrative policy.";
                    IsAutorunEnabled = false;
                });
                break;
            case StartupTaskState.Enabled:
                _dispatcher.Dispatch(() =>
                {
                    IsAutorunEnabled = true;
                });
                break;
        }
    }

    private void EnableAutoRun(StartupTask startupTask)
    {
        var task = startupTask.RequestEnableAsync();
        while (task.Status == AsyncStatus.Started)
        {
            Thread.Sleep(100);
        }
        var status = task.GetResults();
        _dispatcher.Dispatch(() =>
        {
            IsAutorunEnabled = status == StartupTaskState.Enabled;
            if (!IsAutorunEnabled)
            {
                AutorunError = "Autorun could not be activated.";
            }
        });
    }

    private void DisableAutoRun(StartupTask startupTask)
    {
        startupTask.Disable();
        IsAutorunEnabled = false;
    }

    private void StartPageViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IsAutorunEnabled):
                ToggleAutorun();
                break;
        }
    }

    private void ToggleAutorun()
    {
        var task = StartupTask.GetAsync("Bitwarden_Autofill");
        while (task.Status == AsyncStatus.Started)
        {
            Thread.Sleep(100);
        }
        StartupTask startupTask = task.GetResults();
        switch (startupTask.State)
        {
            case StartupTaskState.Disabled when IsAutorunEnabled:
                EnableAutoRun(startupTask);
                break;
            case StartupTaskState.Enabled when !IsAutorunEnabled:
                DisableAutoRun(startupTask);
                break;
        }
    }
}
