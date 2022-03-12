namespace Smp.Testing.Http.Tests;

using Xunit;
using FluentAssertions;
using static Helpers.CacheFolder;

[Trait("MockApiTests", "")]
public class CacheClientTests
{
    readonly Uri[] uris;
    public CacheClientTests()
    {
        uris = GetFromSeedData().Result;
    }

    [Fact(DisplayName = "can load seed urls")]
    public async Task T1()
    {
        var directoryPath = GetCacheDirPath();

        CleanFolder(directoryPath);

        var options = new CacheClientOptions(
            DirectoryPath: directoryPath
        );

        var client = CacheClient.Create(options);

        foreach (var uri in uris)
        {
            await client.GetAsync(uri);
        }

        var numNewFiles = Directory.GetFiles(directoryPath,"*.http", SearchOption.AllDirectories).Length;
        numNewFiles.Should().Be(uris.Length);
    }

    [Fact(DisplayName = "after urls was loaded response should come from cache")]
    public async void T2()
    {
        var directoryPath = GetCacheDirPath();

        //CleanFolder(directoryPath);

        var options = new CacheClientOptions(
            DirectoryPath: directoryPath
        );

        var client = CacheClient.Create(options);

        foreach (var uri in uris)
        {
            await client.GetAsync(uri);
        }

        foreach (var uri in uris)
        {
            var res = await client.GetAsync(uri);
            res.IsFromCache().Should().BeTrue();
        }
    }

    [Fact(DisplayName = "load 1")]
    public async Task T3()
    {
        var directoryPath = GetCacheDirPath();

        var options = new CacheClientOptions(
            DirectoryPath: directoryPath
        );

        var client = CacheClient.Create(options);

        var html = await client.GetStringAsync("https://marksfriggin.blogspot.com/2006/08/tuesdays-howard-stern-radio-show.html");
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
