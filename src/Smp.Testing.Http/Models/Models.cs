namespace Smp.Testing.Http.Models;

using System.Net;

public record HttpFile(RequestSection Request, ResponseSection Response);

public record RequestSection(
    string Url,
    HttpMethod Method,
    Dictionary<string, string> Headers);
public record ResponseSection(
    int StatusCode,
    Dictionary<string, string> Headers,
    string? Content);
