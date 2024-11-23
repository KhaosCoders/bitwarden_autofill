using System.Text.Json;

namespace Bitwarden_Autofill.Bitwarden;

internal static class CliSerializer
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
    }
}