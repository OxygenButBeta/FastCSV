using o2.FastCSV.Extensions;

namespace o2.FastCSV
{
    public class DataRow
    {
        int _rowIndex;
        public DataCell[] Cells { get; private set; }
        public DataTable ParentTable { get; private set; }
        public DataCell this[int index] => Cells[index];
        public DataCell this[string header] => GetCellAt(header);
        public DataRow(object[] Values, int rowIndex, DataTable parentTable)
        {
            Cells = new DataCell[Values.Length];
            _rowIndex = rowIndex;
            ParentTable = parentTable;
            for (int i = 0; i < Values.Length; i++)
                Cells[i] = new DataCell(Values[i], new CellPosition(rowIndex, i), this);

        }
        public DataCell FindCell(int ColumnIndex, Func<DataCell, bool> predicate)
        {
            if (ColumnIndex.IsBetween(0, Cells.Length) || predicate is null)
                throw new ArgumentException("Invalid index or predicate is null");

            return predicate(Cells[ColumnIndex]) ? Cells[ColumnIndex] : null;
        }
        public DataCell GetCellAt(int ColumnIndex) => Cells[ColumnIndex];
        public DataCell GetCellAt(string HeaderName) => GetCellAt(Array.IndexOf(ParentTable.Headers, HeaderName));
        public DataCell FindCell(string HeaderName, Func<DataCell, bool> predicate) =>
            FindCell(Array.IndexOf(ParentTable.Headers, HeaderName), predicate);
        public override string ToString()
        {
            var str = string.Empty;
            object[] values = new object[Cells.Length];
            for (int i = 0; i < Cells.Length; i++)
                values[i] = Cells[i].Value.ToString();
            str += string.Join(" | ", values);
            return str;
        }

        public DataRow? Next() => ParentTable.Rows[_rowIndex + 1];
        public DataRow? Previous() => ParentTable.Rows[_rowIndex - 1];

    }
}
