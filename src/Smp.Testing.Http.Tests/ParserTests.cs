namespace Smp.Testing.Http.Tests;
using Xunit;
using FluentAssertions;
using Smp.Testing.Http.Models;
using System.Net;
using Smp.Testing.Http.Tests.Helpers;

[Trait("parsing", "")]
public class ParserTests
{
    [Fact(DisplayName = "can parse response correctly")]
    public async Task T1()
    {
        var txt = await File.ReadAllTextAsync(@"tests-data\parsing\request.http");
        var req = txt.FromText();
        Assert.NotNull(req);
        req.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        req.Response.Content.Should().Contain("<span>Tuesday, August 01, 2006</span>");
        req.Response.Headers.Count.Should().Be(9);

    }


    [Fact(DisplayName = "can parse multiple test folder")]
    public void T2()
    {
        var files = CacheFolder.GetFiles("marksfriggin.blogspot.com");

        foreach (var file in files.Take(10000))
        {
            var txt = File.ReadAllText(file);
            var http = txt.FromText();
            http.Response.StatusCode.Should().Be(200);
            http.Response.Headers.Count.Should().BePositive();
            http.Request.Headers.Count.Should().Be(0);
        }
    }
}


