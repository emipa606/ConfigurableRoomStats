using System.Diagnostics;
using Mlie;
using SquirtingElephant.Helpers;
using UnityEngine;
using Verse;

namespace SquirtingElephant.ConfigurableRoomStats;

public class SE_Settings : Mod
{
    private const float SCROLL_AREA_OFFSET_TOP = 64f;

    private const float COL_WIDTH = 350f;

    private const float SLIDER_COL_WIDTH = 300f;

    private const float ROW_HEIGHT = 32f;

    private const float BUTTON_Y = 0f;

    private const float BUTTON_HEIGHT = 32f;

    private const float PADDING_HORIZONTAL = 10f;
    public static SettingsData Settings;
    private static string currentVersion;

    public static readonly TableData Table = new TableData(new Vector2(50f, SCROLL_AREA_OFFSET_TOP),
        new Vector2(0f, 2f),
        [COL_WIDTH, SLIDER_COL_WIDTH], [ROW_HEIGHT], -1, 44);

    private Vector2 ScrollPosition = Vector2.zero;

    private Rect ViewRect = new Rect(0f, 0f, 100f, 200f);

    public SE_Settings(ModContentPack content)
        : base(content)
    {
        Settings = GetSettings<SettingsData>();

        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect settingsWindowSizeRect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(settingsWindowSizeRect);
        CreateTopButtons(settingsWindowSizeRect.width);
        Widgets.Label(new Rect(0f, ROW_HEIGHT, settingsWindowSizeRect.width, ROW_HEIGHT),
            "SECRS_AllSettingsAppliedLive".TC());
        if (currentVersion != null)
        {
            listing_Standard.Gap(60f);
            GUI.contentColor = Color.gray;
            listing_Standard.Label("SECRS_CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        Widgets.BeginScrollView(
            viewRect: new Rect(settingsWindowSizeRect.x + PADDING_HORIZONTAL, settingsWindowSizeRect.y,
                settingsWindowSizeRect.width - 20f, 1800f),
            outRect: new Rect(0f, 64f, settingsWindowSizeRect.width,
                settingsWindowSizeRect.height - SCROLL_AREA_OFFSET_TOP),
            scrollPosition: ref ScrollPosition);
        listing_Standard.GetRect(Table.Bottom);
        CreateAllSettings();
        Widgets.EndScrollView();

        listing_Standard.End();
        Main.ApplySettingsToDefs();
    }

    private void CreateAllSettings()
    {
        var num = 0;
        foreach (var key in Settings.StatData.Keys)
        {
            Widgets.Label(Table.GetFieldRect(0, num++), key.HeaderKey().TC());
            foreach (var item in Settings.StatData[key])
            {
                MakeInputs(num++, item.TranslationKey, ref item.Value, item.MinValue, item.MaxValue);
            }

            if (Widgets.ButtonText(Table.GetFieldRect(0, num++).RightHalf(), "SECRS_Reset".TC()))
            {
                Settings.Reset(key);
            }

            num++;
        }
    }

    private void CreateTopButtons(float menuWidth)
    {
        var num = menuWidth / 4f;
        if (Widgets.ButtonText(new Rect(0f, 0f, num, BUTTON_HEIGHT), "SECRS_ApplyPreset".TC()))
        {
            Find.WindowStack.Add(new FloatMenu([
                new FloatMenuOption("SECRS_PresetVanilla".TC(), delegate { ChosePreset(0); }),
                new FloatMenuOption("SECRS_PresetRealistic".TC(), delegate { ChosePreset(1); }),
                new FloatMenuOption("SECRS_PresetMin".TC(), delegate { ChosePreset(2); }),
                new FloatMenuOption("SECRS_PresetMax".TC(), delegate { ChosePreset(3); }),
                new FloatMenuOption("SECRS_PresetCustom1".TC(), delegate { ChosePreset(4); }),
                new FloatMenuOption("SECRS_PresetCustom2".TC(), delegate { ChosePreset(5); })
            ]));
        }

        if (Widgets.ButtonText(new Rect(num, 0f, num, BUTTON_HEIGHT), "SECRS_SavePreset".TC()))
        {
            Find.WindowStack.Add(new FloatMenu([
                new FloatMenuOption($"{"SECRS_SavePreset".TC()} 1",
                    delegate { Settings.SaveToPreset(1); }),

                new FloatMenuOption($"{"SECRS_SavePreset".TC()} 2",
                    delegate { Settings.SaveToPreset(2); })
            ]));
        }

        if (Widgets.ButtonText(new Rect(num * 2f, BUTTON_Y, num, BUTTON_HEIGHT), "SECRS_OpenModSettingsFolder".TC()))
        {
            Process.Start(Utils.GetModSettingsFolderPath());
        }

        if (Widgets.ButtonText(new Rect(num * 3f, BUTTON_Y, num, BUTTON_HEIGHT), "SECRS_ResetAll".TC()))
        {
            Settings.ResetAll();
        }
    }

    private void ChosePreset(int preset)
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

    private void MakeInputs(int rowIdx, string translationKey, ref float setting, float min, float max)
    {
        var buffer = setting.ToString();
        Widgets.TextFieldNumericLabeled(Table.GetFieldRect(0, rowIdx),
            translationKey.Translate().CapitalizeFirst() + " ", ref setting, ref buffer, min, max);
        setting = Widgets.HorizontalSlider(Table.GetFieldRect(1, rowIdx), setting, min, max);
    }

    public override string SettingsCategory()
    {
        return "SECRS_SettingsCategory".Translate();
    }
}