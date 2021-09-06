using Microsoft.Extensions.Configuration;
using SecretSettings.Rewriters.Json;
using SecretSettings.SecretProviders.Aws;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecretSettings.Extensions
{
    public static class IConfigurationBuilderExtensions
    {
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
}
