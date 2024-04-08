using UnityEngine;

namespace SquirtingElephant.Helpers;

public abstract class TableEntity(TableData tableData)
{
    protected readonly TableData TableData = tableData;

    public string Name = string.Empty;

    public Rect Rect { get; set; }
}