using SecretSettings.SecretProviders;
using SecretSettings.SecretProviders.Aws;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecretSettings.Rewriters.Json
{
    internal class SecretRefJsonElementRewriter : JsonElementRewriter
    {
        public const string AwsSecretRefPropertyName = "SecretRef::AWS";

        private readonly ISecretProviderFactory<AwsSecretProviderModel> _awsSecretProviderFactory;

        public SecretRefJsonElementRewriter(ISecretProviderFactory<AwsSecretProviderModel> awsSecretProviderFactory)
        {
            _awsSecretProviderFactory = awsSecretProviderFactory;
        }

        protected override Task RewriteObject(JsonProperty[] jsonProperties, Utf8JsonWriter jsonWriter)
        {
            if (IsAwsSecretRefObject(jsonProperties))
            {
                return RewriteAwsSecretRefObject(jsonProperties, jsonWriter);
            }
            else
            {
                return base.RewriteObject(jsonProperties, jsonWriter);
            }
        }

        private static bool IsAwsSecretRefObject(JsonProperty[] jsonProperties)
        {
            return
            (
                jsonProperties.Length == 1 &&
                jsonProperties[0].Name == AwsSecretRefPropertyName &&
                jsonProperties[0].Value.ValueKind == JsonValueKind.Object
            );
        }

        private async Task RewriteAwsSecretRefObject(JsonProperty[] jsonProperties, Utf8JsonWriter jsonWriter)
        {
            var awsSecretProviderModel = JsonSerializer.Deserialize<AwsSecretProviderModel>(jsonProperties[0].Value.GetRawText()) ?? throw new InvalidOperationException();

            try
            {
                var awsSecretProvider = _awsSecretProviderFactory.Construct();

                var secretObject = await awsSecretProvider.GetSecretObject<JsonElement>(awsSecretProviderModel);

                await RewriteAny(secretObject, jsonWriter);
            }
            catch
            {
                if (awsSecretProviderModel.ThrowOnError)
                {
                    throw;
                }
            }
        }
    }
}
