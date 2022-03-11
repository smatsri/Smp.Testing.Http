namespace Smp.Testing.Http.Models;

using System.Net;
using System.Text;

public static class ModelsExt
{

    public static HttpFile FromText(this string str)
    {
        var (r, s) = str.SplitBy("\r\n\r\n\r\n");
        var request = ParseRequest(r);
        var response = ParseResponse(s);
        return new HttpFile(request, response);
    }

    static RequestSection ParseRequest(string str)
    {
        var reqRows = str.Split('\n');
        var reqUrlParts = reqRows[0].Split(' ');
        var method = reqUrlParts[0].ParseHttpMethod();
        var url = reqUrlParts[1].Trim();
        var headers = reqRows[1..].ParseHeaders();
        return new RequestSection(url, method, headers);
    }
    static ResponseSection ParseResponse(string str)
    {
        var (first, content) = str.SplitBy("\n\r");
        var rows = first.Split('\n');

        var status = rows[0].ParseHttpStatusCode();
        var resHeaders = rows[1..].ParseHeaders();

        return new ResponseSection(status, resHeaders, content);
    }

    static HttpMethod ParseHttpMethod(this string str)
    {
        return str?.ToLower() switch
        {
            "post" => HttpMethod.Post,
            _ => HttpMethod.Get,
        };
    }

    public static (string, string) SplitBy(this string str, string sep)
    {
        var i = str.IndexOf(sep);
        if (i == -1) return (str, string.Empty);
        var f = str[..i];
        var s = str[(i + sep.Length)..];
        return (f, s);
    }

    static Dictionary<string, string> ParseHeaders(this IEnumerable<string> rows)
    {
       var empty = (string.Empty, string.Empty);

        return rows
            .Select(r => ParseHeader(r.Trim()))
            .Where(a => a != empty)
            .GroupBy(a => a.key)
            .ToDictionary(kv => kv.Key, kv => kv.Select(a => a.value).First());

        (string key, string value) ParseHeader(string row)
        {
            var (f,s) = row.SplitBy(":");
            if(string.IsNullOrEmpty(s)) return empty;

            return (f, s);
        }
    }



    static int ParseHttpStatusCode(this string row)
    {
        if (string.IsNullOrEmpty(row))
        {
            return -1;
        }

        int code;
        if (row.StartsWith("HTTP"))
        {
            var parts = row.Split(' ');
            code = int.Parse(parts[1]);
        }
        else
        {
            code = int.Parse(row);
        }


        return code;
    }

    public static string ToText(this HttpFile file)
    {
        var sb = new StringBuilder();

        sb.AppendRequestSection(file.Request);
        sb.AppendSectionSeparator();
        sb.AppendResponseSection(file.Response);

        return sb.ToString();
    }

    static void AppendRequestSection(
        this StringBuilder sb,
        RequestSection section)
    {
        sb.AppendFormat("{0} {1}\n", section.Method.Method, section.Url);
        sb.AppendHeaders(section.Headers);
    }

    static void AppendSectionSeparator(this StringBuilder sb)
    {
        sb.Append("\n\n");
    }
    static void AppendResponseSection(
        this StringBuilder sb,
        ResponseSection section)
    {
        sb.AppendFormat("{0}\n", (int)section.StatusCode);
        sb.AppendHeaders(section.Headers);
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(section.Content))
            sb.Append(section.Content);


    }

    static void AppendHeaders(
        this StringBuilder sb,
        Dictionary<string, string> headers)
    {
        foreach (var header in headers)
        {
            sb.AppendFormat("{0}:{1}\n", header.Key, header.Value);
        }
    }
}