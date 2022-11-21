namespace SecretSettings.SecretProviders
{
    internal interface ISecretProviderFactory<TSecretProviderModel>
    {
        ISecretProvider<TSecretProviderModel> Construct();
    }
}
