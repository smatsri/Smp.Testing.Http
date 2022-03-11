namespace Smp.Testing.Http.Tests;
using Xunit;
using FluentAssertions;
using Smp.Testing.Http.Models;
using System.Net;

[Trait("parsing", "")]
public class ParserTests
{
    [Fact(DisplayName = "can parse http file")]
    public async Task T1()
    {
        var txt = await File.ReadAllTextAsync(@"tests-data\parsing\request.http");
        var req = txt.FromText();
        Assert.NotNull(req);
        req.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        req.Response.Content.Should().Contain("<span>Tuesday, August 01, 2006</span>");
        req.Response.Headers.Count.Should().Be(9);

    }

}
