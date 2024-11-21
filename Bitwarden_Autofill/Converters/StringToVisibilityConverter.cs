using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Bitwarden_Autofill.Converters;

internal class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) =>
        value switch
        {
            string s => string.IsNullOrEmpty(s) ? Visibility.Collapsed : Visibility.Visible,
            _ => Visibility.Collapsed,
        };

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException();
}
