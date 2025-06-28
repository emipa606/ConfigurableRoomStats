using Verse;

namespace SquirtingElephant.Helpers;

public static class Extensions
{
    public static string Tc(this string s)
    {
        return s.Translate().CapitalizeFirst();
    }
}