namespace Smp.Testing.Http.Handlers;

using Models;

class ReaderHandler : HttpMessageHandler
{
    private readonly CacheDirectory cache;

    public ReaderHandler(CacheDirectory cache)
    {
        this.cache = cache;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await Task.Yield();

        var file = await cache.GetFile(request.Method, request.RequestUri);

        if (file == null)
            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);

        return ToHttpResponse(file);

    }


    static HttpResponseMessage ToHttpResponse(HttpFile def)
    {
        const string ContentTypeKey = "Content-Type";

        var res = new HttpResponseMessage
        {
            StatusCode = def.Response.StatusCode
        };

        foreach (var header in def.Response.Headers)
        {
            if (header.Key == ContentTypeKey)
                continue;

            res.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (def.Response.Content != null)
        {
            var mediaType =
                def.Response.Headers.ContainsKey(ContentTypeKey) 
                ? def.Response.Headers[ContentTypeKey] 
                : null;

            res.Content = new StringContent(
                def.Response.Content,
                System.Text.Encoding.UTF8,
                mediaType);
        }

        return res;
    }
}
