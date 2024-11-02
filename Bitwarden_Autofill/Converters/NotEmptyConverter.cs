using Microsoft.UI.Xaml.Data;
using System;

namespace Bitwarden_Autofill.Converters;

internal class NotEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) =>
        value switch
        {
            string s => !string.IsNullOrEmpty(s),
            _ => (object)false,
        };

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException();
}
