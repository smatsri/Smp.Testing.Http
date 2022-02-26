namespace Smp.Testing.Http.Handlers;

class CacheClientHandler : HttpMessageHandler
{
    private readonly CacheDirectory cache;
    private readonly HttpClient client;

    public CacheClientHandler(CacheDirectory cache, HttpClient client)
    {
        this.cache = cache;
        this.client = client;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var file = await cache.GetFile(request.Method, request.RequestUri);

        if (file != null)
            return file.ToHttpResponse();

        var response = await client.SendAsync(request.Clone(), cancellationToken);
        await Save(request, response);

        return response;
    }

    async Task Save(
        HttpRequestMessage request,
        HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            return;

        var httpFile = await Helpers.ToHttpFile(request, response);
        await cache.SaveFile(httpFile);
    }
}
