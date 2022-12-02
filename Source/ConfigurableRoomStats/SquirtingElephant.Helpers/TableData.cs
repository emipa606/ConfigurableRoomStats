using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SquirtingElephant.Helpers;

public class TableData
{
    private const float DEFAULT_ROW_HEIGHT = 32f;

    public readonly bool UpdateEnabled = true;

    private Vector2 _Spacing;

    private Vector2 _TableOffset;

    private bool PrivateUpdateEnabled = true;

    public TableData(Vector2 tableOffset, Vector2 spacing, float[] colWidths, float[] rowHeights, int colCount = -1,
        int rowCount = -1)
    {
        Initialize(tableOffset, spacing, colWidths, rowHeights, colCount, rowCount);
    }

    public float Bottom => TableRect.yMax;

    public Vector2 Spacing
    {
        get => _Spacing;
        set
        {
            if (value == _Spacing)
            {
                return;
            }

            _Spacing = value;
            Update();
        }
    }

    public Vector2 TableOffset
    {
        get => _TableOffset;
        set
        {
            if (value == _TableOffset)
            {
                return;
            }

            _TableOffset = value;
            Update();
        }
    }

    public List<TableColumn> Columns { get; set; } = new List<TableColumn>();

    public List<TableRow> Rows { get; set; } = new List<TableRow>();

    public Rect TableRect { get; private set; } = Rect.zero;

    private void Initialize(Vector2 tableOffset, Vector2 tableSpacing, float[] colWidths, float[] rowHeights,
        int colCount = -1, int rowCount = -1)
    {
        PrivateUpdateEnabled = false;
        _TableOffset = tableOffset;
        _Spacing = tableSpacing;
        var array = colWidths;
        foreach (var colWidth in array)
        {
            AddColumns(colWidth);
        }

        AddColumns(colWidths.Last(), colCount - colWidths.Length);
        array = rowHeights;
        foreach (var rowHeight in array)
        {
            AddRow(rowHeight);
        }

        AddRow(GetLastRowHeight(), rowCount - Rows.Count);
        PrivateUpdateEnabled = true;
        Update();
    }

    public void Update(bool force = false)
    {
        if ((!UpdateEnabled || !PrivateUpdateEnabled) && !force)
        {
            return;
        }

        SetTableRect();
        UpdateColumns();
        UpdateRowsAndFields();
    }

    private float CalcTableWidth()
    {
        if (Columns.Count == 0)
        {
            return 0f;
        }

        var num = Columns[0].Width;
        for (var i = 1; i < Columns.Count; i++)
        {
            num += Columns[i].Width + Spacing.x;
        }

        return num;
    }

    private float CalcTableHeight()
    {
        if (Rows.Count == 0)
        {
            return 0f;
        }

        var num = Rows[0].Height;
        for (var i = 1; i < Rows.Count; i++)
        {
            num += Rows[i].Height + Spacing.y;
        }

        return num;
    }

    private void SetTableRect()
    {
        TableRect = new Rect(TableOffset.x, TableOffset.y, CalcTableWidth(), CalcTableHeight());
    }

    public void UpdateColumns()
    {
        var x = TableRect.x;
        foreach (var column in Columns)
        {
            column.Rect = new Rect(x, TableRect.y, column.Width, TableRect.height);
            x = column.Rect.xMax + Spacing.x;
        }
    }

    public void UpdateRowsAndFields()
    {
        var y = TableRect.y;
        foreach (var row in Rows)
        {
            row.Rect = new Rect(TableRect.x, y, TableRect.width, row.Height);
            y = row.Rect.yMax + Spacing.y;
            row.UpdateFields();
        }
    }

    public void AddRow(float rowHeight, int amount = 1)
    {
        if (amount == 0)
        {
            return;
        }

        PrivateUpdateEnabled = false;
        for (var i = 0; i < amount; i++)
        {
            Rows.Add(new TableRow(this, rowHeight));
        }

        PrivateUpdateEnabled = true;
        Update();
    }

    private void AddColumns(float colWidth, float amount = 1f)
    {
        if (amount == 0f)
        {
            return;
        }

        PrivateUpdateEnabled = false;
        for (var i = 0; i < amount; i++)
        {
            Columns.Add(new TableColumn(this, colWidth));
        }

        PrivateUpdateEnabled = true;
        Update();
    }

    private void CreateRowsUntil(int rowIdx)
    {
        AddRow(GetLastRowHeight(), rowIdx + 1 - Rows.Count);
    }

    public TableField GetField(int colIdx, int rowIdx)
    {
        if (colIdx >= Columns.Count)
        {
            Log.Error($"Attemped to access a column that's out of bounds. Received: {colIdx}.");
            return TableField.Invalid;
        }

        CreateRowsUntil(rowIdx);
        return Rows[rowIdx].Fields[colIdx];
    }

    private float GetLastRowHeight()
    {
        return Rows.Count <= 0 ? 32f : Rows.Last().Height;
    }

    public Rect GetRowRect(int rowIdx)
    {
        CreateRowsUntil(rowIdx);
        return Rows[rowIdx].Rect;
    }

    public Rect GetHeaderRect(int colIdx)
    {
        return GetField(colIdx, 0).Rect;
    }

    public Rect GetFieldRect(int colIdx, int rowIdx)
    {
        return GetField(colIdx, rowIdx).Rect;
    }

    public void ApplyMouseOverEntireRow(int rowIdx)
    {
        var rowRect = GetRowRect(rowIdx);
        if (Mouse.IsOver(rowRect))
        {
            Widgets.DrawHighlight(rowRect);
        }
    }
}
