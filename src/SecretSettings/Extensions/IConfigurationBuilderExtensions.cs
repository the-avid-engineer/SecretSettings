using Microsoft.Extensions.Configuration;
using SecretSettings.Rewriters.Json;
using SecretSettings.SecretProviders.Aws;
using System.Text.Json;

namespace SecretSettings.Extensions;

/// <summary>
///     Extension methods for <see cref="IConfigurationBuilder"/>
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    ///     Add a JSON file with secret refs to the configuration builder.
    /// </summary>
    /// <param name="configurationBuilder">The configuration builder.</param>
    /// <param name="jsonSecretsFileName">The name of the JSON secrets file.</param>
    /// <returns>The configuration builder.</returns>
    public static IConfigurationBuilder AddJsonSecretsFile
    (
        this IConfigurationBuilder configurationBuilder,
        string jsonSecretsFileName
    )
    {
        var secretsStreamTask = Task.Run(async () =>
        {
            var secretsFilePath = Path.Combine(AppContext.BaseDirectory, jsonSecretsFileName);

            await using var secretsFile = File.OpenRead(secretsFilePath);

            var secrets = await JsonSerializer.DeserializeAsync<JsonElement>(secretsFile);

            var awsSecretProviderFactory = new AwsSecretProviderFactory();

            var secretRefJsonElementRewriter = new SecretRefJsonElementRewriter(awsSecretProviderFactory);

            return await secretRefJsonElementRewriter.RewriteAsMemoryStream(secrets);
        });

        secretsStreamTask.Wait();

        return configurationBuilder.AddJsonStream(secretsStreamTask.Result);
    }
}
