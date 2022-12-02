using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using RimWorld;
using UnityEngine;
using Verse;

namespace SquirtingElephant.Helpers;

public static class Utils
{
    public enum ESide
    {
        LeftHalf,
        RightHalf,
        BothSides
    }

    private const string DEF_NOT_FOUND_FORMAT =
        "Unable to find {0}: {1}. Please ensure that this def exists in the database and that the database was loaded before trying to locate this.";

    private const string SINGLE_WHITE_SPACE = " ";

    public static void LogDefNotFoundWarning(string defName, string defType = "Def")
    {
        Log.Warning(
            $"Unable to find {defType}: {defName}. Please ensure that this def exists in the database and that the database was loaded before trying to locate this.");
    }

    public static void LogDefNotFoundError(string defName, string defType = "Def")
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
            LogDefNotFoundError(recipeDefName, typeof(T).Name);
        }

        return null;
    }

    public static void SetThingStat(string thingDefName, string statDefName, float newValue)
    {
        var defByDefName = GetDefByDefName<ThingDef>(thingDefName);
        if (defByDefName != null)
        {
            defByDefName.statBases.Find(s => s.stat.defName == statDefName).value = newValue;
        }
    }

    public static string GetModSettingsFolderPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return
                $"C:\\Users\\{Environment.UserName}\\AppData\\LocalLow\\Ludeon Studios\\RimWorld by Ludeon Studios\\Config";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "~/Library/Application Support/RimWorld/Config";
        }

        return "~/.config/unity3d/Ludeon Studios/RimWorld by Ludeon Studios/Config";
    }

    public static void OpenModSettingsFolder()
    {
        var modSettingsFolderPath = GetModSettingsFolderPath();
        if (Directory.Exists(modSettingsFolderPath))
        {
            Process.Start(modSettingsFolderPath);
        }
        else
        {
            Log.Error(
                $"Unable to open path: {modSettingsFolderPath}. This error is not problematic and doesn't hurt your game.");
        }
    }

    public static void SetupTextureAndColorIt(ref Texture2D texture, int width, int height, Color color)
    {
        var loadedTextures = false;
        if (texture == null)
        {
            texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            loadedTextures = true;
        }
        else if (texture.width != width || texture.height != height)
        {
            if (width == 0 || height == 0)
            {
                Log.Error("Received either a 0 for the texture width and/or height.");
            }

            loadedTextures = texture.width < width || texture.height < height;
            texture.width = width;
            texture.height = height;
        }

        if (!loadedTextures)
        {
            return;
        }

        var pixels = texture.GetPixels();
        for (var i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }

    public static IEnumerable<Building> GetBuildingsByDefName(string defName)
    {
        if (Current.Game == null || Current.Game.CurrentMap == null)
        {
            return Enumerable.Empty<Building>();
        }

        return Current.Game.CurrentMap.listerBuildings.allBuildingsColonist.Where(b => b.def.defName == defName);
    }

    public static void AddRecipeUnique(ThingDef thingDef, RecipeDef recipe)
    {
        if (thingDef.recipes.Any(r => r.defName == recipe.defName))
        {
            thingDef.recipes.Add(recipe);
        }
    }

    public static void CopyRecipesFromAToB(string sourceDefName, string destinationDefName)
    {
        var defByDefName = GetDefByDefName<ThingDef>(sourceDefName);
        var defByDefName2 = GetDefByDefName<ThingDef>(destinationDefName);
        foreach (var recipe in defByDefName.recipes)
        {
            AddRecipeUnique(defByDefName2, recipe);
        }
    }

    public static void AddRecipesToDef(string thingDefName, bool errorOnRecipeNotFound, params string[] recipeDefNames)
    {
        if (recipeDefNames.Length == 0)
        {
            return;
        }

        var defByDefName = GetDefByDefName<ThingDef>(thingDefName, false);
        if (defByDefName == null)
        {
            return;
        }

        foreach (var recipeDefName in recipeDefNames)
        {
            var defByDefName2 = GetDefByDefName<RecipeDef>(recipeDefName, errorOnRecipeNotFound);
            if (defByDefName2 != null)
            {
                AddRecipeUnique(defByDefName, defByDefName2);
            }
        }
    }

    private static Rect GetRectFor(Listing_Standard ls, ESide side, float rowHeight)
    {
        return side switch
        {
            ESide.LeftHalf => ls.GetRect(rowHeight).LeftHalf(),
            ESide.RightHalf => ls.GetRect(rowHeight).RightHalf(),
            ESide.BothSides => ls.GetRect(rowHeight),
            _ => throw new ArgumentException("Unexpected value", nameof(side))
        };
    }

    public static void MakeCheckboxLabeled(Listing_Standard ls, string translationKey, ref bool checkedSetting,
        ESide side = ESide.RightHalf, float rowHeight = 32f)
    {
        Widgets.CheckboxLabeled(GetRectFor(ls, side, rowHeight), translationKey.Translate().CapitalizeFirst() + " ",
            ref checkedSetting);
    }

    public static void MakeTextFieldNumericLabeled<T>(Listing_Standard ls, string translationKey, ref T setting,
        float min = 1f, float max = 1000f, ESide side = ESide.RightHalf, float rowHeight = 32f) where T : struct
    {
        var rectFor = GetRectFor(ls, side, rowHeight);
        var buffer = setting.ToString();
        Widgets.TextFieldNumericLabeled(rectFor, translationKey.Translate().CapitalizeFirst() + " ", ref setting,
            ref buffer, min, max);
    }

    public static void EditPowerGenerationValue(string thingDefName, int newPowerGenerationAmount)
    {
        var defByDefName = GetDefByDefName<ThingDef>(thingDefName);
        if (defByDefName != null)
        {
            defByDefName.comps.OfType<CompProperties_Power>().First().basePowerConsumption =
                -Math.Abs(newPowerGenerationAmount);
        }
    }

    public static void SetWorkAmount(string recipeDefName, int newWorkAmount)
    {
        var defByDefName = GetDefByDefName<RecipeDef>(recipeDefName);
        if (defByDefName != null)
        {
            defByDefName.workAmount = newWorkAmount;
        }
    }

    public static void SetYieldAmount(string recipeDefName, int newYieldAmount)
    {
        GetDefByDefName<RecipeDef>(recipeDefName)?.products.ForEach(delegate(ThingDefCountClass p)
        {
            p.count = newYieldAmount;
        });
    }

    public static void SetResearchBaseCost(string researchDefName, int newResearchCost)
    {
        var defByDefName = GetDefByDefName<ResearchProjectDef>(researchDefName);
        if (defByDefName != null)
        {
            defByDefName.baseCost = newResearchCost;
        }
    }

    public static void SetThingMaxHp(string thingDefName, int newHP)
    {
        SetThingStat(thingDefName, "MaxHitPoints", newHP);
    }

    public static void SetThingMaxBeauty(string thingDefName, int newBeauty)
    {
        SetThingStat(thingDefName, "Beauty", newBeauty);
    }

    public static void SetThingTurretBurstCooldown(string thingDefName, float newTurretBurstCooldown)
    {
        var defByDefName = GetDefByDefName<ThingDef>(thingDefName);
        if (defByDefName != null)
        {
            defByDefName.building.turretBurstCooldownTime = newTurretBurstCooldown;
        }
    }

    public static void SetThingSteelCost(string thingDefName, int newSteelCost)
    {
        var defByDefName = GetDefByDefName<ThingDef>(thingDefName);
        if (defByDefName == null)
        {
            return;
        }

        var thingDefCountClass =
            Enumerable.FirstOrDefault(defByDefName.costList, c => c.thingDef == ThingDefOf.Steel);
        if (thingDefCountClass != null)
        {
            thingDefCountClass.count = newSteelCost;
        }
    }

    public static void SetThingComponentCost(string thingDefName, int newComponentCost)
    {
        var defByDefName = GetDefByDefName<ThingDef>(thingDefName);
        if (defByDefName == null)
        {
            return;
        }

        var thingDefCountClass = Enumerable.FirstOrDefault(defByDefName.costList,
            c => c.thingDef == ThingDefOf.ComponentIndustrial);
        if (thingDefCountClass != null)
        {
            thingDefCountClass.count = newComponentCost;
        }
    }

    public static void SetThingComponentSpacerCost(string thingDefName, int newComponentSpacerCost)
    {
        var defByDefName = GetDefByDefName<ThingDef>(thingDefName);
        if (defByDefName == null)
        {
            return;
        }

        var thingDefCountClass =
            Enumerable.FirstOrDefault(defByDefName.costList, c => c.thingDef == ThingDefOf.ComponentSpacer);
        if (thingDefCountClass != null)
        {
            thingDefCountClass.count = newComponentSpacerCost;
        }
    }
}
