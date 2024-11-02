using Microsoft.Extensions.Options;
using Microsoft.Win32;

namespace Bitwarden_Autofill.Options;

internal class SettingsStore : IOptionsFactory<AppSettings>, ISettingsStore
{
    private const string RegistryKeyHotkey = @"Software\Bitwarden_Autofill\GlobalHotkey";
    private const string DefaultHotkey = "Ctrl + Shift + B";

    public AppSettings Create(string name)
    {
        return new AppSettings
        {
            GlobalHotkey = Registry.CurrentUser.GetValue(RegistryKeyHotkey, DefaultHotkey).ToString()
        };
    }

    public void StoreGlobalHotkey(string hotkey)
    {
        Registry.CurrentUser.SetValue(RegistryKeyHotkey, hotkey);
    }
}
