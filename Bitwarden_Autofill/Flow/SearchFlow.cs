using Bitwarden_Autofill.Bitwarden;
using Bitwarden_Autofill.View;
using System.Threading;

namespace Bitwarden_Autofill.Flow;

internal class SearchFlow(UIDispatcher dispatcher, VaultSearch vaultSearch)
{
    public async Task SearchAsync(string searchText, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;
        var foundItems = await vaultSearch.FindItems(searchText);

        if (cancellationToken.IsCancellationRequested) return;
        dispatcher.ShowPage<SelectItemPage>(page =>
        {
            lock (page.ViewModel)
            {
                if (cancellationToken.IsCancellationRequested) return;
                page.ViewModel.Items.Clear();
                foreach (var item in foundItems)
                {
                    page.ViewModel.Items.Add(item);
                }
            }
        });
    }
}
