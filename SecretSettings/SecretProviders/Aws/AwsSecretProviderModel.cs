namespace SecretSettings.SecretProviders.Aws
{
    internal class AwsSecretProviderModel
    {
        public string? SecretId { get; set; }
        public string? VersionStage { get; set; }
        public string? VersionId { get; set; }
        public bool ThrowOnError { get; set; } = true;
    }
}
