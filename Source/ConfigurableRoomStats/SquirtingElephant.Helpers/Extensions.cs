using Verse;

namespace SquirtingElephant.Helpers;

public static class Extensions
{
    public static string TC(this string s)
    {
        return s.Translate().CapitalizeFirst();
    }
}