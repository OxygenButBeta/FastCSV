using o2.FastCSV.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace o2.FastCSV
{
    public static class DataTableExtensions
    {
        public static DataCell GetCellAt(this DataTable table, CellPosition position) => table.GetCellAt(position.X, position.Y);
        public static DataCell GetCellAt(this DataTable table, int x, int y) => table.Rows[x].Cells[y];
        public static IEnumerable<DataCell> FindValuesInAnywhere(this DataTable table, object value)
        {
            List<DataCell> cells = new();

            for (int _ = 0; _ < table.Columns.Count; _++)
                cells.AddRange(table.Columns[_].FindValues(value));

            return cells;
        }
        public static IEnumerable<DataCell> GetAllFromColumn(this DataTable table, int ColumnIndex)
        {
            if (!ColumnIndex.IsBetween(0, table.Headers.Length))
                throw new IndexOutOfRangeException("The column index is out of range");

            return table.Rows.Select(row => row.Cells[ColumnIndex]);
        }
        public static DataCell FindFirstValueInAnywhere(this DataTable table, object value)
        {
            for (int i = 0; i < table.Rows.Count; i++)
                for (int j = 0; j < table.Rows[i].Cells.Length; j++)
                    if (table.GetCellAt(i, j).Equals(value))
                        return table.Rows[i].Cells[j];

            return null;
        }
        public static void PrintTable(this DataTable table, int PrintAmount = -1)
        {
            Console.WriteLine(string.Join(" | ", table.Headers));

            int TotalRows = PrintAmount == -1 || PrintAmount > table.Rows.Count ? table.Rows.Count : PrintAmount;

            for (int i = 0; i < TotalRows; i++)
                Console.WriteLine(table.Rows[i]);
        }

    }
}
