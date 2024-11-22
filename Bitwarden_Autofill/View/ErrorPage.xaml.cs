using Bitwarden_Autofill.ViewModel;
using Microsoft.UI.Xaml.Controls;

namespace Bitwarden_Autofill.View;

/// <summary>
/// A page that shows an error message.
/// </summary>
internal sealed partial class ErrorPage : Page
{
    public ErrorPage()
    {
        InitializeComponent();
        ViewModel = new();
    }

    public ErrorViewModel ViewModel { get; set; }
}
