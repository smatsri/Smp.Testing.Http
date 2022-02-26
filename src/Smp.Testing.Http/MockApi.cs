namespace Smp.Testing.Http;

using Handlers;

public class MockApi
{
    private readonly CacheDirectory directory;
    private readonly HttpClient reader;
    public MockApi(string directoryPath, string? baseAddress = null)
    {
        directory = new CacheDirectory(directoryPath);
        reader = CreateRead(baseAddress);
    }

    public HttpClient Reader => reader;

    HttpClient CreateRead(
        string? baseAddress = null)
    {

        var handler = new ReaderHandler(directory);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseAddress ?? "https://localhost")
        };

        return httpClient;
    }

    public HttpClient CreateWrite(
        string baseAddress)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
        var handler = new WriterHandler(client, directory);

        return new HttpClient(handler)
        {
            BaseAddress = client.BaseAddress
        };
    }

    public bool Exists(string uri, HttpMethod? method = null) => directory.Exists(method ?? HttpMethod.Get, uri);
}
