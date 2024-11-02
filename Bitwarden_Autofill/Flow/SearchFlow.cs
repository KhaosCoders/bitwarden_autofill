using Bitwarden_Autofill.API;
using Bitwarden_Autofill.View;
using System.Threading;

namespace Bitwarden_Autofill.Flow;

internal class SearchFlow(UIDispatcher dispatcher, BitwardenApi api)
{
    public async Task SearchAsync(string searchText, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;
        var foundItems = await api.FindItems(searchText);

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
