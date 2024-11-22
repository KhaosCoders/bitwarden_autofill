using Bitwarden_Autofill.API;
using Bitwarden_Autofill.CLI;
using Bitwarden_Autofill.Flow;
using Bitwarden_Autofill.Hotkey;
using Bitwarden_Autofill.Options;
using Bitwarden_Autofill.View;
using Bitwarden_Autofill.Win;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.UI.Dispatching;

namespace Bitwarden_Autofill;

public partial class App
{
    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ClientFactory>();

        services.AddSingleton(DispatcherQueue.GetForCurrentThread());
        services.AddSingleton<UIDispatcher>();

        services.AddSingleton<GlobalHotkey>();
        services.AddSingleton<WinProcesses>();
        services.AddSingleton<WindowPlacement>();

        services.AddSingleton<IOptionsFactory<AppSettings>, SettingsStore>();
        services.AddSingleton<ISettingsStore, SettingsStore>();
        services.AddOptions<AppSettings>();

        services.AddSingleton<BitwardenCli>();
        services.AddSingleton<BitwardenApi>();
        services.AddTransient<DownloadCliWindow>();
        services.AddTransient<MainWindow>();
        services.AddTransient<ErrorPage>();
        services.AddTransient<StartPage>();
        services.AddTransient<LoadingPage>();
        services.AddTransient<LoginPage>();
        services.AddTransient<UnlockPage>();
        services.AddTransient<SelectItemPage>();

        services.AddSingleton<AppFlow>();
        services.AddSingleton<BitwardenCliDownloadFlow>();
        services.AddSingleton<BitwardenCliServeFlow>();
        services.AddSingleton<LoginFlow>();
        services.AddSingleton<PasswordFlow>();
        services.AddSingleton<SearchFlow>();

        return services.BuildServiceProvider();
    }
}
