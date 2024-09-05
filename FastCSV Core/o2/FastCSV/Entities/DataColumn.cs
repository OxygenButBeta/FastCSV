using o2.FastCSV.Extensions;
using System.Collections;

namespace o2.FastCSV
{
    public class DataColumn : IEnumerator, IEnumerable
    {
        // Fields
        DataTable ParentTable;
        int xIndex;
        string _header = "Unassigned";

        // Properties
        public DataCell this[int yIndex] => ParentTable.GetCellAt(xIndex, yIndex);
        public string Header
        {
            get => _header;
            set
            {
                var oldHeader = _header;
                if (ParentTable.CaptureLogs)
                    if (_header != value)
                        ParentTable.ChangeLogs.Add($"Column header changed from '{oldHeader}' to '{value}'");

                _header = value;
            }
        }


        // Constructors
        public DataColumn(DataTable parentTable, string header, int xIndex)
        {
            ParentTable = parentTable;
            _header = header;
            this.xIndex = xIndex;
        }

        // Methods
        public IEnumerable<DataCell> GetCells()
        {
            List<DataCell> cells = new List<DataCell>();
            foreach (var row in ParentTable.Rows)
                cells.Add(row.Cells[xIndex]);

            return cells;
        }
        public DataCell FindValue(object value) => Internal_Search(value, true).FirstOrDefault();
        public IEnumerable<DataCell> FindValues(object value) => Internal_Search(value);
        internal IEnumerable<DataCell> Internal_Search(object value, bool Single = false)
        {
            List<DataCell> cells = new List<DataCell>();

            for (int i = 0; i < ParentTable.Rows.Count; i++)
                if (ParentTable.GetCellAt(i, xIndex).Value.Equals(value))
                {
                    if (Single) return new List<DataCell> { ParentTable.Rows[i].Cells[xIndex] };
                    cells.Add(ParentTable.Rows[i].Cells[xIndex]);
                }

            return cells;
        }

        #region IEnumerator
        public object Current { get; private set; }
        int CurrentIndex = 0;
        public bool MoveNext()
        {
            if (CurrentIndex + 1 >= ParentTable.Rows.Count)
                return false;

            CurrentIndex++;
            try
            {
                Current = ParentTable.GetCellAt(CurrentIndex, xIndex);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public void Reset() => CurrentIndex = 0;
        public IEnumerator GetEnumerator() => this;
        #endregion

    }
}
