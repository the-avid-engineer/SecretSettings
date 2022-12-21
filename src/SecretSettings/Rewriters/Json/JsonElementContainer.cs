using System.Text.Json;

namespace SecretSettings.Rewriters.Json;

internal record JsonElementContainer : IElementContainer<JsonElementContainer>
{
    public required JsonElement Value { get; init; }

    public static JsonElementContainer Deserialize(string data)
    {
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(data);

        return new JsonElementContainer { Value = jsonElement };
    }
}
