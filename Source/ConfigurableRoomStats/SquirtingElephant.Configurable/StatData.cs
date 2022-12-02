using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SquirtingElephant.ConfigurableRoomStats;

public class StatData : IExposable
{
    private const string PREFIX = "SECRS_";

    private static readonly List<(float, float)> MinMaxConsts = new List<(float, float)>
    {
        (2f, 2500f),
        (-1000f, 1000f),
        (1f, 1000000f),
        (1f, 2500f),
        (-5f, 1f)
    };

    public float CustomPreset1;

    public float CustomPreset2;

    public float MaxValue;

    public float MinValue;

    public List<float> Presets;

    public float Value;

    public StatData(EStatType statType, string translationKeyNoPrefix, params float[] presets)
    {
        StatType = statType;
        TranslationKey = $"SECRS_{translationKeyNoPrefix}";
        Value = presets[0];
        Presets = presets.ToList();
        CustomPreset1 = CustomPreset2 = Presets[0];
        MinValue = MinMaxConsts[(int)StatType].Item1;
        MaxValue = MinMaxConsts[(int)StatType].Item2;
    }

    public EStatType StatType { get; }

    public string TranslationKey { get; }

    public void ExposeData()
    {
        Scribe_Values.Look(ref Value, $"{TranslationKey}_Value", Presets[0], true);
        Scribe_Values.Look(ref CustomPreset1, $"{TranslationKey}_CustomPreset1", Presets[0], true);
        Scribe_Values.Look(ref CustomPreset2, $"{TranslationKey}_CustomPreset2", Presets[0], true);
    }

    public void CreateDefaultPresets()
    {
        Presets.Add(MinValue);
        Presets.Add(MaxValue);
    }

    public void DuplicateVanillaPreset()
    {
        DuplicatePreset(0);
    }

    public void DuplicatePreset(int presetIdx)
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
                Value = CustomPreset1;
                break;
            default:
                Value = CustomPreset2;
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
            CustomPreset1 = Value;
        }
        else
        {
            CustomPreset2 = Value;
        }
    }
}
