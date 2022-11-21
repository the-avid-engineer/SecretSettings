using Amazon.Runtime;
using Amazon.SecretsManager;

namespace SecretSettings.SecretProviders.Aws
{
    internal class AwsSecretProviderFactory : ISecretProviderFactory<AwsSecretProviderModel>
    {
        public ISecretProvider<AwsSecretProviderModel> Construct()
        {
            var awsCredentials = FallbackCredentialsFactory.GetCredentials();
            var regionEndpoint = FallbackRegionFactory.GetRegionEndpoint();

            var amazonSecretsManagerClient = new AmazonSecretsManagerClient(awsCredentials, regionEndpoint);

            return new AwsSecretProvider(amazonSecretsManagerClient);
        }
    }
}
