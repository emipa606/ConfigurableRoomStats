using SquirtingElephant.Helpers;
using Verse;

namespace SquirtingElephant.ConfigurableRoomStats;

[StaticConstructorOnStartup]
public class Main
{
    static Main()
    {
        if (SE_Settings.Settings.StatData == null)
        {
            SE_Settings.Settings.CreateStatData();
        }

        ApplySettingsToDefs();
    }

    private static void ApplyScores(EStatType statType)
    {
        var defByDefName = Utils.GetDefByDefName<RoomStatDef>(statType.RoomStatDefName());
        if (defByDefName == null)
        {
            return;
        }

        for (var i = 0; i < SE_Settings.Settings.StatData[statType].Count; i++)
        {
            defByDefName.scoreStages[i + 1].minScore = SE_Settings.Settings.StatData[statType][i].Value;
        }
    }

    public static void ApplySettingsToDefs()
    {
        ApplyScores(EStatType.Space);
        ApplyScores(EStatType.Beauty);
        ApplyScores(EStatType.Wealth);
        ApplyScores(EStatType.Imp);
        ApplyScores(EStatType.Clean);
    }
}
