using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecretSettings.Rewriters.Json
{
    public class JsonElementRewriter : IRewriter<JsonElement>
    {
        public async Task Rewrite(JsonElement jsonElement, Stream outputStream)
        {
            await using var jsonWriter = new Utf8JsonWriter(outputStream);

            await RewriteAny(jsonElement, jsonWriter);

            await jsonWriter.FlushAsync();
        }

        protected Task RewriteAny(JsonElement jsonElement, Utf8JsonWriter jsonWriter)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.Array => RewriteArray(jsonElement.EnumerateArray().ToArray(), jsonWriter),
                JsonValueKind.Object => RewriteObject(jsonElement.EnumerateObject().ToArray(), jsonWriter),
                _ => RewriteScalar(jsonElement, jsonWriter),
            };
        }

        protected virtual async Task RewriteArray(JsonElement[] jsonElements, Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartArray();

            foreach (var childElement in jsonElements)
            {
                await RewriteAny(childElement, jsonWriter);
            }

            jsonWriter.WriteEndArray();
        }

        protected virtual async Task RewriteObject(JsonProperty[] jsonProperties, Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();

            foreach (var jsonProperty in jsonProperties)
            {
                jsonWriter.WritePropertyName(jsonProperty.Name);

                await RewriteAny(jsonProperty.Value, jsonWriter);
            }

            jsonWriter.WriteEndObject();
        }

        protected virtual Task RewriteScalar(JsonElement jsonElement, Utf8JsonWriter jsonWriter)
        {
            jsonElement.WriteTo(jsonWriter);

            return Task.CompletedTask;
        }
    }
}
