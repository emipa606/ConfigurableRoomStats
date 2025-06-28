using System.Diagnostics;
using Mlie;
using SquirtingElephant.Helpers;
using UnityEngine;
using Verse;

namespace SquirtingElephant.ConfigurableRoomStats;

public class SE_Settings : Mod
{
    private const float ScrollAreaOffsetTop = 64f;

    private const float ColWidth = 350f;

    private const float SliderColWidth = 300f;

    private const float RowHeight = 32f;

    private const float ButtonY = 0f;

    private const float ButtonHeight = 32f;

    private const float PaddingHorizontal = 10f;
    public static SettingsData Settings;
    private static string currentVersion;

    private static readonly TableData table = new(new Vector2(50f, ScrollAreaOffsetTop),
        new Vector2(0f, 2f),
        [ColWidth, SliderColWidth], [RowHeight], -1, 44);

    private Vector2 scrollPosition = Vector2.zero;

    private Rect viewRect = new(0f, 0f, 100f, 200f);

    public SE_Settings(ModContentPack content)
        : base(content)
    {
        Settings = GetSettings<SettingsData>();

        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect settingsWindowSizeRect)
    {
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(settingsWindowSizeRect);
        createTopButtons(settingsWindowSizeRect.width);
        Widgets.Label(new Rect(0f, RowHeight, settingsWindowSizeRect.width, RowHeight),
            "SECRS_AllSettingsAppliedLive".Tc());
        if (currentVersion != null)
        {
            listingStandard.Gap(60f);
            GUI.contentColor = Color.gray;
            listingStandard.Label("SECRS_CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        Widgets.BeginScrollView(
            viewRect: new Rect(settingsWindowSizeRect.x + PaddingHorizontal, settingsWindowSizeRect.y,
                settingsWindowSizeRect.width - 20f, 1800f),
            outRect: new Rect(0f, 64f, settingsWindowSizeRect.width,
                settingsWindowSizeRect.height - ScrollAreaOffsetTop),
            scrollPosition: ref scrollPosition);
        listingStandard.GetRect(table.Bottom);
        createAllSettings();
        Widgets.EndScrollView();

        listingStandard.End();
        Main.ApplySettingsToDefs();
    }

    private void createAllSettings()
    {
        var num = 0;
        foreach (var key in Settings.StatData.Keys)
        {
            Widgets.Label(table.GetFieldRect(0, num++), key.HeaderKey().Tc());
            foreach (var item in Settings.StatData[key])
            {
                MakeInputs(num++, item.TranslationKey, ref item.Value, item.MinValue, item.MaxValue);
            }

            if (Widgets.ButtonText(table.GetFieldRect(0, num++).RightHalf(), "SECRS_Reset".Tc()))
            {
                Settings.Reset(key);
            }

            num++;
        }
    }

    private void createTopButtons(float menuWidth)
    {
        var num = menuWidth / 4f;
        if (Widgets.ButtonText(new Rect(0f, 0f, num, ButtonHeight), "SECRS_ApplyPreset".Tc()))
        {
            Find.WindowStack.Add(new FloatMenu([
                new FloatMenuOption("SECRS_PresetVanilla".Tc(), delegate { ChosePreset(0); }),
                new FloatMenuOption("SECRS_PresetRealistic".Tc(), delegate { ChosePreset(1); }),
                new FloatMenuOption("SECRS_PresetMin".Tc(), delegate { ChosePreset(2); }),
                new FloatMenuOption("SECRS_PresetMax".Tc(), delegate { ChosePreset(3); }),
                new FloatMenuOption("SECRS_PresetCustom1".Tc(), delegate { ChosePreset(4); }),
                new FloatMenuOption("SECRS_PresetCustom2".Tc(), delegate { ChosePreset(5); })
            ]));
        }

        if (Widgets.ButtonText(new Rect(num, 0f, num, ButtonHeight), "SECRS_SavePreset".Tc()))
        {
            Find.WindowStack.Add(new FloatMenu([
                new FloatMenuOption($"{"SECRS_SavePreset".Tc()} 1",
                    delegate { Settings.SaveToPreset(1); }),

                new FloatMenuOption($"{"SECRS_SavePreset".Tc()} 2",
                    delegate { Settings.SaveToPreset(2); })
            ]));
        }

        if (Widgets.ButtonText(new Rect(num * 2f, ButtonY, num, ButtonHeight), "SECRS_OpenModSettingsFolder".Tc()))
        {
            Process.Start(Utils.GetModSettingsFolderPath());
        }

        if (Widgets.ButtonText(new Rect(num * 3f, ButtonY, num, ButtonHeight), "SECRS_ResetAll".Tc()))
        {
            Settings.ResetAll();
        }
    }

    private static void ChosePreset(int preset)
    {
        if (Settings.StatData == null)
        {
            Log.Error("ChosePreset() detected a null-value for Settings.StatData.");
            return;
        }

        foreach (var statDatum in Settings.StatData)
        {
            statDatum.Value.ForEach(delegate(StatData s) { s.ApplyPreset(preset); });
        }
    }

    private static void MakeInputs(int rowIdx, string translationKey, ref float setting, float min, float max)
    {
        var buffer = setting.ToString();
        Widgets.TextFieldNumericLabeled(table.GetFieldRect(0, rowIdx),
            translationKey.Translate().CapitalizeFirst() + " ", ref setting, ref buffer, min, max);
        setting = Widgets.HorizontalSlider(table.GetFieldRect(1, rowIdx), setting, min, max);
    }

    public override string SettingsCategory()
    {
        return "SECRS_SettingsCategory".Translate();
    }
}