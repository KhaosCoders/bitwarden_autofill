using System.Collections.Generic;

namespace Bitwarden_Autofill.API.Models;

public class ListResult<TTemplate>
{
    public List<TTemplate>? Data { get; set; }
}
