using Bitwarden_Autofill.API.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bitwarden_Autofill.ViewModel;

internal partial class SelectItemViewModel : ObservableObject
{
	[ObservableProperty]
	private string search = string.Empty;

	public ObservableCollection<BitwardenItem> Items { get; } = [];

	[ObservableProperty]
	private BitwardenItem? selectedItem;

	public ICommand? UsernameCommand { get; set; }
	public ICommand? PasswordCommand { get; set; }
	public ICommand? TotpCommand { get; set; }

    public delegate void ItemSelectedEventHandler(BitwardenItem? item);
    public event ItemSelectedEventHandler? ItemSelected;

    public delegate void SearchChanged(string text);
    public event SearchChanged? SearchTextChanged;

    partial void OnSelectedItemChanged(BitwardenItem? value)
    {
        ItemSelected?.Invoke(value);
    }

    partial void OnSearchChanged(string value)
    {
        SearchTextChanged?.Invoke(value);
    }
}
