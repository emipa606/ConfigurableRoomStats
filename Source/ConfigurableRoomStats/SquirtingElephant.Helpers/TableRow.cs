using System.Collections.Generic;
using Verse;

namespace SquirtingElephant.Helpers;

public class TableRow : TableEntity
{
    private float height;

    public TableRow(TableData tableData, float height)
        : base(tableData)
    {
        Height = height;
        setFields(tableData);
    }

    public List<TableField> Fields { get; } = [];

    public float Height
    {
        get => height;
        private set
        {
            if (value == height)
            {
                return;
            }

            if (value > 0f)
            {
                height = value;
                TableData.Update();
            }
            else
            {
                Log.Error($"TableRow received a value of {value} for its Height.");
            }
        }
    }

    private void setFields(TableData tableData)
    {
        Fields.Clear();
        tableData.Columns.ForEach(delegate(TableColumn c) { Fields.Add(new TableField(tableData, c, this)); });
    }

    public void UpdateFields()
    {
        Fields.ForEach(delegate(TableField f) { f.Update(); });
    }
}