using System;

namespace SquirtingElephant.Helpers;

public static class SeMath
{
    public static int RoundToNearestMultiple(int value, float multiple)
    {
        return (int)(Math.Round(value / multiple) * multiple);
    }

    public static float CalcColumn_X(float offset_X, float colWidth, float spacing_X, int colIdx)
    {
        return offset_X + ((colWidth + spacing_X) * colIdx);
    }
}
