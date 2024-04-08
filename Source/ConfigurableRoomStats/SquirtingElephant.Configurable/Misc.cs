using System.Collections.Generic;

namespace SquirtingElephant.ConfigurableRoomStats;

public static class Misc
{
    private static readonly List<string> HeaderKeys =
    [
        "SECRS_RoomSpaceHeader", "SECRS_RoomBeautyHeader", "SECRS_RoomWealthHeader", "SECRS_RoomImpHeader",
        "SECRS_RoomCleanHeader"
    ];

    private static readonly List<string> RoomStatDefNames =
        ["Space", "Beauty", "Wealth", "Impressiveness", "Cleanliness"];

    public static string HeaderKey(this EStatType statType)
    {
        return HeaderKeys[(int)statType];
    }

    public static string RoomStatDefName(this EStatType statType)
    {
        return RoomStatDefNames[(int)statType];
    }
}