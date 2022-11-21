using System.Threading.Tasks;

namespace SecretSettings.SecretProviders
{
    internal interface ISecretProvider<TSecretProviderModel>
    {
        Task<TSecretObject?> GetSecretObject<TSecretObject>(TSecretProviderModel secretProviderModel);
    }
}
