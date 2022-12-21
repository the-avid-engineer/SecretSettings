using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using SecretSettings.Rewriters;
using System.Collections.Concurrent;
using System.Text;

namespace SecretSettings.SecretProviders.Aws;

internal class AwsSecretProvider : ISecretProvider<AwsSecretProviderModel>
{
    private readonly ConcurrentDictionary<string, string> _cache = new();
    private readonly IAmazonSecretsManager _amazonSecretsManager;

    public AwsSecretProvider(IAmazonSecretsManager amazonSecretsManager)
    {
        _amazonSecretsManager = amazonSecretsManager;
    }

    private async Task<string> GetSerializedSecret(AwsSecretProviderModel awsProviderModel)
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

        return serializedSecret;
    }

    private async Task<string> GetCachedSerializedSecret(AwsSecretProviderModel awsProviderModel)
    {
        var cacheKey = awsProviderModel.ToCacheKey();

        if (!_cache.TryGetValue(cacheKey, out var serializedSecret) || serializedSecret is null)
        {
            serializedSecret = await GetSerializedSecret(awsProviderModel);

            _cache.TryAdd(cacheKey, serializedSecret);
        }

        return serializedSecret;
    }

    public async Task<TElementContainer> GetSecretObject<TElementContainer>(AwsSecretProviderModel awsProviderModel)
        where TElementContainer : IElementContainer<TElementContainer>
    {
        return TElementContainer.Deserialize(await GetCachedSerializedSecret(awsProviderModel));
    }
}
