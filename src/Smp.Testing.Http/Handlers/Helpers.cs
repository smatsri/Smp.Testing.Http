namespace Smp.Testing.Http.Handlers;

using Models;
using System.Net;

static class Helpers
{
    public static RequestSection ToRequestSection(
        this HttpRequestMessage request)
    {
        if (request.RequestUri == null)
            throw new ArgumentNullException(nameof(request));

        var url = request.RequestUri.ToString();
        var method = request.Method;
        var headers = new Dictionary<string, string>();

        foreach (var header in request.Headers)
        {
            var value = string.Join(";", header.Value);
            headers.Add(header.Key, value);
        }

        return new RequestSection(url, method, headers);
    }

    public static async Task<ResponseSection> ToResponseSection(this HttpResponseMessage response)
    {
        var status = response.StatusCode;

        var headers = new Dictionary<string, string>();
        foreach (var header in response.Headers)
        {
            var value = string.Join(";", header.Value);
            headers.Add(header.Key, value);
        }

        var content = string.Empty;
        if (status == System.Net.HttpStatusCode.OK)
        {
            content = await response.Content.ReadAsStringAsync();
        }

        return new ResponseSection((int)status, headers, content);
    }

    public static HttpRequestMessage Clone(this HttpRequestMessage req)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri)
        {
            Content = req.Content,
            Version = req.Version
        };

        foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
        {
            try
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            catch
            {
                continue;

            }
        }

        return clone;
    }

    public static HttpResponseMessage ToHttpResponse(this HttpFile file)
    {
        const string ContentTypeKey = "Content-Type";

        var res = new HttpResponseMessage
        {
            StatusCode = (HttpStatusCode)file.Response.StatusCode
        };

        res.Headers.TryAddWithoutValidation(Consts.CacheHeader, "");


        foreach (var header in file.Response.Headers)
        {
            if (header.Key == ContentTypeKey)
                continue;

            res.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }


        if (file.Response.Content != null)
        {
            var mediaType =
                file.Response.Headers.ContainsKey(ContentTypeKey)
                ? file.Response.Headers[ContentTypeKey]
                : null;

            res.Content = new StringContent(
                file.Response.Content,
                System.Text.Encoding.UTF8,
                mediaType);
        }

        return res;
    }

    public static async Task<HttpFile> ToHttpFile(HttpRequestMessage request,
        HttpResponseMessage response)
    {
        var reqSection = request.ToRequestSection();
        var resSection = await response.ToResponseSection();
        return new HttpFile(reqSection, resSection);
    }

    public static Task<HttpFile> ToHttpFile(this Pair pair) 
        => ToHttpFile(pair.Request, pair.Response);

    public static  Task<HttpFile> ToHttpFile(
        this Tuple<HttpRequestMessage, HttpResponseMessage> pair)
    {
        var (r,s) = pair;
        return ToHttpFile(r, s);
    }

    public record Pair(
        HttpRequestMessage Request,
        HttpResponseMessage Response);
}