namespace Smp.Testing.Http.Models;

using System.Text;

public static class ModelsExt
{
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