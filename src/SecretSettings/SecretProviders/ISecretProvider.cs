using SecretSettings.Rewriters;

namespace SecretSettings.SecretProviders;

internal interface ISecretProvider<TSecretProviderModel>
{
    Task<TElementContainer> GetSecretObject<TElementContainer>(TSecretProviderModel secretProviderModel)
        where TElementContainer : IElementContainer<TElementContainer>;
}
