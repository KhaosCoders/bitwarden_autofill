namespace Bitwarden_Autofill.Options;

internal interface ISettingsStore
{
    void StoreGlobalHotkey(string hotkey);
}
