namespace Smp.Testing.Http.Models;

using System.Net;
using System.Text;

static class ModelsExt
{

    public static HttpFile FromText(this string str)
    {
        var rows = str.Split('\n');
        var parts = rows.SplitByEmptyRow(2).ToArray();
        var reqRows = parts[0];
        var resRows = parts[1..].Aggregate((a, b) => a.Concat(b).ToArray());

        var reqUrlParts = reqRows[0].Split(' ');
        var method = reqUrlParts[0].ParseHttpMethod();
        var url = reqUrlParts[1].Trim();
        var headers = reqRows[1..].ParseHeaders();

        var resParts = resRows.SplitByEmptyRow(1).ToArray();
        var metadata = resParts[0];
        var status = metadata[0].ParseHttpStatusCode();
        var resHeaders = metadata[1..].ParseHeaders();
        var aa = resParts[1..].Aggregate((a, b) => a.Concat(b).ToArray());
        var content = string.Join('\n', aa).Trim();

        return new HttpFile(
            new RequestSection(url, method, headers),
            new ResponseSection(status, resHeaders, content)
        );
    }

    static HttpMethod ParseHttpMethod(this string str)
    {
        return str?.ToLower() switch
        {
            "post" => HttpMethod.Post,
            _ => HttpMethod.Get,
        };
    }

    static IEnumerable<string[]> SplitByEmptyRow(this IEnumerable<string> rows, int numRowSep = 1)
    {
        var lst = new List<string>();
        var count = 0;
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row))
            {
                count++;
                if (count == numRowSep)
                {
                    lst.RemoveRange(lst.Count - count + 1, count - 1);
                    yield return lst.ToArray();
                    count = 0;
                    lst = new List<string>();
                    continue;
                }


            }

            lst.Add(row);
        }
        if (lst.Count > 0)
            yield return lst.ToArray();
    }

    static Dictionary<string, string> ParseHeaders(this IEnumerable<string> rows)
    {
        return rows.Select(r => r.ParseHeader()).ToDictionary(kv => kv.key, kv => kv.value);
    }

    static (string key, string value) ParseHeader(this string row)
    {
        var parts = row.Split(':');
        return (parts[0].Trim(), parts[1].Trim());
    }

    static HttpStatusCode ParseHttpStatusCode(this string row)
    {
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


        return (HttpStatusCode)code;
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