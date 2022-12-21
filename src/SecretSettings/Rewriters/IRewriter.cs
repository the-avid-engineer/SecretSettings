namespace SecretSettings.Rewriters;

internal interface IRewriter<TDocument>
{
    public Task Rewrite(TDocument inputDocument, Stream outputStream);
}
