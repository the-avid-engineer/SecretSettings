using System.Text.Json;

namespace SecretSettings.SecretProviders.Aws;

internal record AwsSecretProviderModel
{
    public required string SecretId { get; init; }
    public string? VersionStage { get; init; }
    public string? VersionId { get; init; }
    public JsonElement? Default { get; init; }

    internal string ToCacheKey()
    {
        return $"{SecretId}:{VersionStage}:{VersionId}";
    }
}
