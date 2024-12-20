using System;
using System.Text.Json.Serialization;

namespace Bitwarden_Autofill.Bitwarden.Model;

/// <summary>
/// Represents the status of the Bitwarden CLI.
/// </summary>
internal class CliStatus
{
    public string? ServerUrl { get; set; }
    public DateTime? LastSync { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EBitwardenStatus Status { get; set; }
}
