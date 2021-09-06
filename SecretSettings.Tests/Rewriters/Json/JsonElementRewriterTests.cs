using SecretSettings.Extensions;
using SecretSettings.Rewriters.Json;
using Shouldly;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SecretSettings.Tests.Rewriters.Json
{
    public class JsonElementRewriterTests
    {
        [Fact]
        public async Task GiveObject_WhenRewriting_ThenOriginalJsonMatchesRewrittenJson()
        {
            // ARRANGE

            var someValue = new
            {
                myArray = new[]
                {
                    true,
                    false,
                },
                myObject = new
                {
                    foo = "bar"
                },
                myScalar = 1234
            };

            var expectedOutput = JsonSerializer.Serialize(someValue);

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(expectedOutput);

            // ACT

            var outputStream = await new JsonElementRewriter().RewriteAsMemoryStream(jsonElement);

            var actualOutput = Encoding.UTF8.GetString(outputStream.ToArray());

            // ASSERT 

            actualOutput.ShouldBe(expectedOutput);
        }
    }
}
