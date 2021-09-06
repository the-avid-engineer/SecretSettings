using SecretSettings.Rewriters;
using System.IO;
using System.Threading.Tasks;

namespace SecretSettings.Extensions
{
    public static class IRewriterExtensions
    {
        public static async Task<MemoryStream> RewriteAsMemoryStream<TDocument>(this IRewriter<TDocument> rewriter, TDocument inputDocument)
        {
            var outputStream = new MemoryStream();

            await rewriter.Rewrite(inputDocument, outputStream);

            return outputStream;
        }
    }
}
