using System;
using System.Data;

namespace AnotherCsvLib.Tests
{
    public static class DtTestExtensions
    {
        public static DataRow Modify(this DataRow row, Action<DataRow> rowModifier)
        {
            rowModifier(row);
            return row;
        }

        public static readonly object SkipColumn = new object();

        public static void InsertRow(this DataTable table, params object[] columns)
        {
            table.Rows.Add(table.NewRow().Modify(row =>
            {
                for (var i = 0; i < columns.Length; i++)
                {
                    if (columns[i] == SkipColumn)
                        continue;
                    row[i] = columns[i];
                }
            }));
        }

        public static void InsertColumns(this DataTable table, params string[] columnNames)
        {
            foreach (var columnName in columnNames)
                table.Columns.Add(columnName);
        }
    }
}