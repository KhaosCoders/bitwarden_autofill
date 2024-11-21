using Bitwarden_Autofill.API.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace Bitwarden_Autofill.ViewModel;

internal partial class SelectItemViewModel : ObservableObject
{
	public ObservableCollection<BitwardenItem> Items { get; } = [];

	[ObservableProperty]
	private BitwardenItem? selectedItem;

    [ObservableProperty]
    private string attachedProcess = string.Empty;

	public ICommand? UsernameCommand { get; set; }
	public ICommand? PasswordCommand { get; set; }
	public ICommand? TotpCommand { get; set; }
    public ICommand? ToggleLinkCommand { get; set; }

    public delegate void ItemSelectedEventHandler(BitwardenItem? item);
    public event ItemSelectedEventHandler? ItemSelected;

    partial void OnSelectedItemChanged(BitwardenItem? value)
    {
        ItemSelected?.Invoke(value);
    }
}
