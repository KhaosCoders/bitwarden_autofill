using System;
using System.Text.Json.Serialization;

namespace Bitwarden_Autofill.API.Models;

public class BitwardenStatus
{
    public string? ServerUrl { get; set; }
    public DateTime? LastSync { get; set; }
    public string? UserEmail { get; set; }
    public string? UserId { get; set; }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EBitwardenStatus Status { get; set; }
}
