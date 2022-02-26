namespace Smp.Testing.Http.Tests;

using Xunit;
using FluentAssertions;

[Trait("MockApiTests", "")]
public class MockApiTests
{
    readonly Uri[] uris;
    public MockApiTests()
    {
        uris = GetFromSeedData().Result;
    }

    [Fact(DisplayName = "can load seed urls")]
    public async Task T1()
    {
        var directoryPath = GetCacheDirPath();
        var baseAddress = @$"{uris[0].Scheme}:\\{uris[0].Host}";

        CleanFolder(directoryPath);

        var options = new CacheClientOptions(
            BaseAddress: baseAddress,
            DirectoryPath: directoryPath
        );

        var client = CacheClient.Create(options);

        foreach (var u in uris)
        {
            var uri = u.PathAndQuery;
            await client.GetAsync(uri);
        }

        var numNewFiles = Directory.GetFiles(directoryPath).Length;
        numNewFiles.Should().Be(uris.Length);
    }

    [Fact(DisplayName = "after urls was loaded response should come from cache")]
    public async void T2()
    {
        var directoryPath = GetCacheDirPath();
        var baseAddress = @$"{uris[0].Scheme}:\\{uris[0].Host}";

        CleanFolder(directoryPath);

        var options = new CacheClientOptions(
            BaseAddress: baseAddress,
            DirectoryPath: directoryPath
        );

        var client = CacheClient.Create(options);

        foreach (var u in uris)
        {
            var uri = u.PathAndQuery;
            await client.GetAsync(uri);
        }

        foreach (var u in uris)
        {
            var uri = u.PathAndQuery;
            var res = await client.GetAsync(uri);
            res.IsFromCache().Should().BeTrue();
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

    static void CleanFolder(string path)
    {
        var di = new DirectoryInfo(path);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
    }
}
