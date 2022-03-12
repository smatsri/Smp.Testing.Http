namespace Smp.Testing.Http.Tests.Helpers;

public static class CacheFolder
{
    public static string GetCacheDirPath()
    {
        var s = Environment.CurrentDirectory;
        s = string.Join('\\', s.Split('\\').ToArray().TakeWhile(a => a != "bin"));
        return Path.Combine(s, "cache");
    }

    public static string[] GetFiles(string folder)
    {
        var path = GetCacheDirPath();
        path = Path.Combine(path, folder);

        return Directory.GetFiles(path);
    }

}


