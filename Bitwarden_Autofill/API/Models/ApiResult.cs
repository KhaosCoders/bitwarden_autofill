namespace Bitwarden_Autofill.API.Models;

public class ApiResult<TData>
{
    public bool Status { get; set; }

    public TData? Data { get; set; }
}
