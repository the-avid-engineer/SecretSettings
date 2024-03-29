﻿using SecretSettings.Rewriters;

namespace SecretSettings.Extensions;

internal static class IRewriterExtensions
{
    public static async Task<MemoryStream> RewriteAsMemoryStream<TDocument>(this IRewriter<TDocument> rewriter, TDocument inputDocument)
    {
        var outputStream = new MemoryStream();

        await rewriter.Rewrite(inputDocument, outputStream);

        outputStream.Seek(0, SeekOrigin.Begin);

        return outputStream;
    }
}
