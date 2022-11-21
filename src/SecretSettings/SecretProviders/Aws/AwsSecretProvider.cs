using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecretSettings.SecretProviders.Aws
{
    internal class AwsSecretProvider : ISecretProvider<AwsSecretProviderModel>
    {
        private readonly IAmazonSecretsManager _amazonSecretsManager;

        public AwsSecretProvider(IAmazonSecretsManager amazonSecretsManager)
        {
            _amazonSecretsManager = amazonSecretsManager;
        }

        public async Task<TSecretObject?> GetSecretObject<TSecretObject>(AwsSecretProviderModel awsProviderModel)
        {
            var secretRequest = new GetSecretValueRequest
            {
                SecretId = awsProviderModel.SecretId,
                VersionStage = awsProviderModel.VersionStage,
                VersionId = awsProviderModel.VersionId,
            };

            var secretResponse = await _amazonSecretsManager.GetSecretValueAsync(secretRequest);

            string serializedSecret;

            if (secretResponse.SecretString != null)
            {
                serializedSecret = secretResponse.SecretString;
            }
            else
            {
                using var secretBinaryStream = secretResponse.SecretBinary;
                using var secretBinaryStreamReader = new StreamReader(secretBinaryStream);

                var secretBinary = await secretBinaryStreamReader.ReadToEndAsync();

                serializedSecret = Encoding.UTF8.GetString(Convert.FromBase64String(secretBinary));
            }

            var secretObject = JsonSerializer.Deserialize<TSecretObject>(serializedSecret);

            return secretObject;
        }
    }
}
