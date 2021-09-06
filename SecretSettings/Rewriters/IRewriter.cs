using System.IO;
using System.Threading.Tasks;

namespace SecretSettings.Rewriters
{
    public interface IRewriter<TDocument>
    {
        public Task Rewrite(TDocument inputDocument, Stream outputStream);
    }
}
