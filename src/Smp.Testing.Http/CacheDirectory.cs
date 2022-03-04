namespace Smp.Testing.Http;

using Models;
using System.Collections.Concurrent;

class CacheDirectory
{
    private readonly string directoryPath;

    public CacheDirectory(string directoryPath)
    {
        this.directoryPath = directoryPath;
    }

    public async Task<HttpFile[]> GetFiles()
    {
        var files = Directory.GetFiles(directoryPath, "*.http");
        var bag = new ConcurrentBag<HttpFile>();
        await Parallel.ForEachAsync(files, async (path, ct) =>
        {
            var http = await GetFile(path);
            if (http == null)
                return;
            bag.Add(http);
        });

        return bag.ToArray();
    }

    public async Task<HttpFile?> GetFile(HttpMethod method, Uri? uri)
    {
        if (uri == null) return null;
        var filePath = GetPath(method, uri);
        return await GetFile(filePath);
    }

    public static async Task<HttpFile?> GetFile(string path)
    {
        if (!File.Exists(path))
            return null;

        var txt = await File.ReadAllTextAsync(path);
        var (r,s) = txt.FromText();

        r.Headers.Add(Consts.CacheHeader, path);

        return new(r, s);
    }

    public async Task SaveFile(HttpFile file)
    {
        var path = GetPath(file.Request.Method, new Uri(file.Request.Url));
        var txt = file.ToText();
        var fileInfo = new FileInfo(path);
        fileInfo.Directory?.Create();
        await File.WriteAllTextAsync(path, txt);
    }

    public bool Exists(HttpMethod method, Uri uri)
    {
        var path = GetPath(method, uri);
        return File.Exists(path);
    }

    string GetPath(HttpMethod method, Uri uri)
    {
        var fileName = Path.Combine(MakeValidFileName(uri.Host), $"{ MakeValidFileName(uri.PathAndQuery)}.{method.Method}.http");
        return Path.Combine(directoryPath, fileName);
    }

    public static string MakeValidFileName(string name)
    {
        string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        var fn = System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        fn = fn.StartsWith('_') ? fn[1..] : fn;
        if (string.IsNullOrEmpty(fn))
        {
            fn = "index";
        }
        return fn;
    }
}
