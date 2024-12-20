using Bitwarden_Autofill.Bitwarden.Model;
using Microsoft.UI.Xaml;
using System;
using System.Linq;
using WinUI.Redemption;

namespace Bitwarden_Autofill.Converters;

internal class AttachedProcessConverter : IMultiValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object[] values, object parameter)
    {
        string processName = (string)values[0];
        BitwardenItem item = (BitwardenItem)values[1];

        bool isAttached = item.Login?.Uris?.Any(uri => uri.Uri?.Equals($"file://{processName}", StringComparison.OrdinalIgnoreCase) == true) == true;

        return (Invert ? !isAttached : isAttached) ? Visibility.Visible : Visibility.Collapsed;
    }
}
