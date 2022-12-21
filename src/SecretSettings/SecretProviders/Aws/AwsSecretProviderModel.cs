namespace SecretSettings.SecretProviders.Aws;

internal record AwsSecretProviderModel
{
    public required string SecretId { get; init; }
    public string? VersionStage { get; init; }
    public string? VersionId { get; init; }
    public bool ThrowOnError { get; init; } = true;
}
