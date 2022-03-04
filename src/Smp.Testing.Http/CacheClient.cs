namespace Smp.Testing.Http;

using Handlers;

public static class CacheClient
{
    public static HttpClient Create(CacheClientOptions options)
    {
        var fallbackClient = new HttpClient();

        var cache = new CacheDirectory(options.DirectoryPath);

        var handler = new CacheClientHandler(cache, fallbackClient);

        return new HttpClient(handler);
    }

    public static bool IsFromCache(this HttpResponseMessage response) 
        => response.Headers.Contains(Consts.CacheHeader);
}

public record CacheClientOptions(string DirectoryPath);
