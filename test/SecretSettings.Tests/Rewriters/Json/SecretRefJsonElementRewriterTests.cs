using Moq;
using SecretSettings.Extensions;
using SecretSettings.Rewriters.Json;
using SecretSettings.SecretProviders;
using SecretSettings.SecretProviders.Aws;
using Shouldly;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SecretSettings.Tests.Rewriters.Json
{
    public class SecretRefJsonElementRewriterTests
    {
        [Fact]
        public async Task GivenSecrets_WhenRewriting_ThenSecretRefsAreReplacedWithSecrets()
        {
            // ARRANGE

            const string SecretId = "My-Secret-Id";
            const string SecretValue = "Bar";

            var secrets = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new
            {
                AwsFoo = new Dictionary<string, object>
                {
                    [SecretRefJsonElementRewriter.AwsSecretRefPropertyName] = new
                    {
                        SecretId,
                    }
                }
            }));

            var expectedOutput = JsonSerializer.Serialize(new
            {
                AwsFoo = SecretValue
            });

            var awsSecretProviderMock = new Mock<ISecretProvider<AwsSecretProviderModel>>(MockBehavior.Strict);

            awsSecretProviderMock
                .Setup(provider => provider.GetSecretObject<JsonElement>(It.IsAny<AwsSecretProviderModel>()))
                .ReturnsAsync(JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(SecretValue)));

            var awsSecretProviderFactoryMock = new Mock<ISecretProviderFactory<AwsSecretProviderModel>>(MockBehavior.Strict);

            awsSecretProviderFactoryMock
                .Setup(factory => factory.Construct())
                .Returns(awsSecretProviderMock.Object);

            // ACT

            var outputStream = await new SecretRefJsonElementRewriter(awsSecretProviderFactoryMock.Object).RewriteAsMemoryStream(secrets);

            var actualOutput = Encoding.UTF8.GetString(outputStream.ToArray());

            // ASSERT

            actualOutput.ShouldBe(expectedOutput);
        }
    }
}
