namespace Smp.Testing.Http.Models;

using System.Net;

record HttpFile(RequestSection Request, ResponseSection Response);

record RequestSection(
    string Url,
    HttpMethod Method,
    Dictionary<string, string> Headers);
record ResponseSection(
    HttpStatusCode StatusCode,
    Dictionary<string, string> Headers,
    string? Content);
