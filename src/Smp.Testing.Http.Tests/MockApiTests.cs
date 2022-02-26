namespace Smp.Testing.Http.Tests;

using Xunit;
using FluentAssertions;

[Trait("MockApiTests", "")]
public class MockApiTests
{
    readonly MockApi mockApi;
    readonly Uri[] uris;
    public MockApiTests()
    {
        mockApi = new MockApi(GetCacheDirPath());
        uris = GetFromSeedData().Result;
    }

    [Fact(DisplayName = "1. can load seed urls")]
    public async Task T1()
    {
        var baseAddress = @$"{uris[0].Scheme}:\\{uris[0].Host}";
        var writer = mockApi.CreateWrite(baseAddress);
        foreach (var u in uris)
        {
            var uri = u.PathAndQuery;
            await writer.GetStringAsync(uri);

            var exists = mockApi.Exists(uri);

            exists.Should().BeTrue();

        }
    }

    [Fact(DisplayName = "2. can read seed urls from cache")]
    public async Task T2()
    {
        var reader = mockApi.Reader;

        foreach (var uri in uris)
        {
            var respose = await reader.GetStringAsync(uri);
            Assert.NotNull(respose);
        }
    }

    static string GetCacheDirPath()
    {
        var s = Environment.CurrentDirectory;
        s = string.Join('\\', s.Split('\\').ToArray().TakeWhile(a => a != "bin"));
        return Path.Combine(s, "cache");
    }

    static async Task<Uri[]> GetFromSeedData()
    {
        var path = Path.Combine("tests-data", "seed.txt");
        var lines = await File.ReadAllLinesAsync(path);
        return lines.Select(line => new Uri(line)).ToArray();
    }
}
