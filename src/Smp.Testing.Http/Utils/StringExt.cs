namespace Smp.Testing.Http.Utils;
internal static class StringExt
{
    public static (string, string) SplitFirst(this string str, char sep, int skip = 0)
    {
        int i = 0;
        do
        {
            i = str.IndexOf(sep, i);

            i++;
        } while (i > 0);

        var f = str[..i];
        var s = str[(i + 1)..].TrimStart();
        return (f, s);
    }


}

