using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SquirtingElephant.Helpers;

public class TableData
{
    private const float DefaultRowHeight = 32f;

    private const bool UpdateEnabled = true;

    private bool privateUpdateEnabled = true;

    public TableData(Vector2 tableOffset, Vector2 spacing, float[] colWidths, float[] rowHeights, int colCount = -1,
        int rowCount = -1)
    {
        initialize(tableOffset, spacing, colWidths, rowHeights, colCount, rowCount);
    }

    public float Bottom => TableRect.yMax;

    private Vector2 Spacing { get; set; }

    private Vector2 TableOffset { get; set; }

    public List<TableColumn> Columns { get; } = [];

    private List<TableRow> Rows { get; } = [];

    private Rect TableRect { get; set; } = Rect.zero;

    private void initialize(Vector2 tableOffset, Vector2 tableSpacing, float[] colWidths, float[] rowHeights,
        int colCount = -1, int rowCount = -1)
    {
        privateUpdateEnabled = false;
        TableOffset = tableOffset;
        Spacing = tableSpacing;
        var array = colWidths;
        foreach (var colWidth in array)
        {
            addColumns(colWidth);
        }

        addColumns(colWidths.Last(), colCount - colWidths.Length);
        array = rowHeights;
        foreach (var rowHeight in array)
        {
            addRow(rowHeight);
        }

        addRow(getLastRowHeight(), rowCount - Rows.Count);
        privateUpdateEnabled = true;
        Update();
    }

    public void Update(bool force = false)
    {
        if ((!UpdateEnabled || !privateUpdateEnabled) && !force)
        {
            return;
        }

        setTableRect();
        updateColumns();
        updateRowsAndFields();
    }

    private float calcTableWidth()
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

    private float calcTableHeight()
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

    private void setTableRect()
    {
        TableRect = new Rect(TableOffset.x, TableOffset.y, calcTableWidth(), calcTableHeight());
    }

    private void updateColumns()
    {
        var x = TableRect.x;
        foreach (var column in Columns)
        {
            column.Rect = new Rect(x, TableRect.y, column.Width, TableRect.height);
            x = column.Rect.xMax + Spacing.x;
        }
    }

    private void updateRowsAndFields()
    {
        var y = TableRect.y;
        foreach (var row in Rows)
        {
            row.Rect = new Rect(TableRect.x, y, TableRect.width, row.Height);
            y = row.Rect.yMax + Spacing.y;
            row.UpdateFields();
        }
    }

    private void addRow(float rowHeight, int amount = 1)
    {
        if (amount == 0)
        {
            return;
        }

        privateUpdateEnabled = false;
        for (var i = 0; i < amount; i++)
        {
            Rows.Add(new TableRow(this, rowHeight));
        }

        privateUpdateEnabled = true;
        Update();
    }

    private void addColumns(float colWidth, float amount = 1f)
    {
        if (amount == 0f)
        {
            return;
        }

        privateUpdateEnabled = false;
        for (var i = 0; i < amount; i++)
        {
            Columns.Add(new TableColumn(this, colWidth));
        }

        privateUpdateEnabled = true;
        Update();
    }

    private void createRowsUntil(int rowIdx)
    {
        addRow(getLastRowHeight(), rowIdx + 1 - Rows.Count);
    }

    private TableField getField(int colIdx, int rowIdx)
    {
        if (colIdx >= Columns.Count)
        {
            Log.Error($"Attemped to access a column that's out of bounds. Received: {colIdx}.");
            return TableField.Invalid;
        }

        createRowsUntil(rowIdx);
        return Rows[rowIdx].Fields[colIdx];
    }

    private float getLastRowHeight()
    {
        return Rows.Count <= 0 ? 32f : Rows.Last().Height;
    }

    public Rect GetFieldRect(int colIdx, int rowIdx)
    {
        return getField(colIdx, rowIdx).Rect;
    }
}