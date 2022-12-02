using UnityEngine;

namespace SquirtingElephant.Helpers;

public abstract class TableEntity
{
    protected readonly TableData TableData;

    public string Name = string.Empty;

    public TableEntity(TableData tableData)
    {
        TableData = tableData;
    }

    public Rect Rect { get; set; }
}
