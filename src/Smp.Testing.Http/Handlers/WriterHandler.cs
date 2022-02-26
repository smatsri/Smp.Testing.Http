namespace Smp.Testing.Http.Handlers;

using Models;

class WriterHandler : HttpMessageHandler
{
    private readonly HttpClient client;
    private readonly CacheDirectory loader;

    public WriterHandler(HttpClient client, CacheDirectory loader)
    {
        this.client = client;
        this.loader = loader;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        
        var res = await client.SendAsync(request.Clone(), cancellationToken);

        var s = await Save(request, res);

        return s;
    }

    public async Task<HttpResponseMessage> Save(
        HttpRequestMessage request,
        HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            return response;

        var httpFile = await ToHttpFile(request, response);
        await loader.SaveFile(httpFile);
        return response;
    }

    static async Task<HttpFile> ToHttpFile(HttpRequestMessage request,
        HttpResponseMessage response)
    {
        var reqSection = request.ToRequestSection();
        var resSection = await response.ToResponseSection();
        return new HttpFile(reqSection, resSection);
    }
}