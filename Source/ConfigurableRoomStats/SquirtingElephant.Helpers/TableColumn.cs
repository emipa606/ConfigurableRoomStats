using Verse;

namespace SquirtingElephant.Helpers;

public class TableColumn : TableEntity
{
    private float width;

    public TableColumn(TableData tableData, float width)
        : base(tableData)
    {
        Width = width;
    }

    public float Width
    {
        get => width;
        private set
        {
            if (value == width)
            {
                return;
            }

            if (value > 0f)
            {
                width = value;
                TableData.Update();
            }
            else
            {
                Log.Error($"TableRow received a value of {value} for its Height.");
            }
        }
    }
}