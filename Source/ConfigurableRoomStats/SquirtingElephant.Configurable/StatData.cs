using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SquirtingElephant.ConfigurableRoomStats;

public class StatData : IExposable
{
    private static readonly List<(float, float)> minMaxConsts =
    [
        (2f, 2500f),
        (-1000f, 1000f),
        (1f, 1000000f),
        (1f, 2500f),
        (-5f, 1f)
    ];

    public readonly float MaxValue;

    public readonly float MinValue;

    public readonly List<float> Presets;

    private float customPreset1;

    private float customPreset2;

    public float Value;

    public StatData(EStatType statType, string translationKeyNoPrefix, params float[] presets)
    {
        StatType = statType;
        TranslationKey = $"SECRS_{translationKeyNoPrefix}";
        Value = presets[0];
        Presets = presets.ToList();
        customPreset1 = customPreset2 = Presets[0];
        MinValue = minMaxConsts[(int)StatType].Item1;
        MaxValue = minMaxConsts[(int)StatType].Item2;
    }

    public EStatType StatType { get; }

    public string TranslationKey { get; }

    public void ExposeData()
    {
        Scribe_Values.Look(ref Value, $"{TranslationKey}_Value", Presets[0], true);
        Scribe_Values.Look(ref customPreset1, $"{TranslationKey}_CustomPreset1", Presets[0], true);
        Scribe_Values.Look(ref customPreset2, $"{TranslationKey}_CustomPreset2", Presets[0], true);
    }

    public void CreateDefaultPresets()
    {
        Presets.Add(MinValue);
        Presets.Add(MaxValue);
    }

    public void DuplicateVanillaPreset()
    {
        duplicatePreset(0);
    }

    private void duplicatePreset(int presetIdx)
    {
        Presets.Add(Presets[presetIdx]);
    }

    public void ApplyPreset(int preset)
    {
        switch (preset)
        {
            case < 4:
                Value = Presets[preset];
                break;
            case 4:
                Value = customPreset1;
                break;
            default:
                Value = customPreset2;
                break;
        }
    }

    public void Reset()
    {
        Value = Presets[0];
    }

    public void SaveToPreset(int customPresetNumber)
    {
        if (customPresetNumber == 1)
        {
            customPreset1 = Value;
        }
        else
        {
            customPreset2 = Value;
        }
    }
}