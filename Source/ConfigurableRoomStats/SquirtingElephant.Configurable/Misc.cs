using System.Collections.Generic;

namespace SquirtingElephant.ConfigurableRoomStats;

public static class Misc
{
    private static readonly List<string> headerKeys =
    [
        "SECRS_RoomSpaceHeader", "SECRS_RoomBeautyHeader", "SECRS_RoomWealthHeader", "SECRS_RoomImpHeader",
        "SECRS_RoomCleanHeader"
    ];

    private static readonly List<string> roomStatDefNames =
        ["Space", "Beauty", "Wealth", "Impressiveness", "Cleanliness"];

    public static string HeaderKey(this EStatType statType)
    {
        return headerKeys[(int)statType];
    }

    public static string RoomStatDefName(this EStatType statType)
    {
        return roomStatDefNames[(int)statType];
    }
}