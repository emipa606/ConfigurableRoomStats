using System;
using System.IO;
using System.Runtime.InteropServices;
using Verse;

namespace SquirtingElephant.Helpers;

public static class Utils
{
    private static void logDefNotFoundError(string defName, string defType = "Def")
    {
        Log.Error(
            $"Unable to find {defType}: {defName}. Please ensure that this def exists in the database and that the database was loaded before trying to locate this.");
    }

    public static T GetDefByDefName<T>(string recipeDefName, bool errorOnNotFound = true) where T : Def
    {
        var named = DefDatabase<T>.GetNamed(recipeDefName);
        if (named != null)
        {
            return named;
        }

        if (errorOnNotFound)
        {
            logDefNotFoundError(recipeDefName, typeof(T).Name);
        }

        return null;
    }

    public static string GetModSettingsFolderPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow",
                "Ludeon Studios", "RimWorld by Ludeon Studios", "Config");
        }

        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? "~/Library/Application Support/RimWorld/Config"
            : "~/.config/unity3d/Ludeon Studios/RimWorld by Ludeon Studios/Config";
    }
}