using System.Collections.Generic;
using Verse;

namespace SquirtingElephant.Helpers;

public class TableRow : TableEntity
{
    private float _Height;

    public TableRow(TableData tableData, float height)
        : base(tableData)
    {
        Height = height;
        SetFields(tableData);
    }

    public List<TableField> Fields { get; } = [];

    public float Height
    {
        get => _Height;
        set
        {
            if (value == _Height)
            {
                return;
            }

            if (value > 0f)
            {
                _Height = value;
                TableData.Update();
            }
            else
            {
                Log.Error($"TableRow received a value of {value} for its Height.");
            }
        }
    }

    private void SetFields(TableData tableData)
    {
        Fields.Clear();
        tableData.Columns.ForEach(delegate(TableColumn c) { Fields.Add(new TableField(tableData, c, this)); });
    }

    public void UpdateFields()
    {
        Fields.ForEach(delegate(TableField f) { f.Update(); });
    }
}