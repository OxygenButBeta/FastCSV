using o2.FastCSV.Extensions;
using System.Text;

namespace o2.FastCSV
{
    public class DataTable
    {
        public enum CsvSource
        {
            File,
            Web,
            NotSet
        }
        // Fields
        string _CsvPath;
        bool _CsvLoaded;


        //Indexers
        /// <summary>
        /// Gets the row at the specified index.
        /// </summary>
        /// <param name="index">Target row Index</param>
        /// <returns></returns>
        public DataRow this[int index] => Rows[index];
        /// <summary>
        /// Gets the column with the specified header.
        /// </summary>
        /// <param name="header">Name of the column</param>
        /// <returns></returns>
        public DataColumn this[string header]
        {
            get
            {
                var result = Columns.Find(x => x.Header == header);
                return result ?? throw new KeyNotFoundException("The column with the specified header was not found.");
            }
        }
        /// <summary>
        /// Gets the cell at the specified position.
        /// </summary>
        /// <param name="x">Row Index</param>
        /// <param name="y">Column Index</param>
        /// <returns></returns>
        public DataCell this[int x, int y] => GetCellAt(x, y);

        // Properties
        public CsvSource _Source { get; private set; } = CsvSource.NotSet;
        public string[] Headers => Columns.Select(x => x.Header).ToArray();
        public List<DataRow> Rows { get; private set; }
        public List<DataColumn> Columns { get; private set; }
        public bool CheckCellEqualityAsString { get; set; } = true;
        public bool CaptureLogs { get; set; } = true;
        public readonly List<string> ChangeLogs;
        public DataTable(bool captureLogs = false)
        {
            if (CaptureLogs)
                ChangeLogs = new List<string>();

            CaptureLogs = captureLogs;
        }


        public DataCell FindValueInAnywhere(object value)
        {
            for (int i = 0; i < Rows.Count; i++)
                for (int j = 0; j < Rows[i].Cells.Length; j++)
                    if (GetCellAt(i, j).Equals(value))
                        return Rows[i].Cells[j];

            return null;
        }
        public IEnumerable<DataCell> FindValuesInAnywhere(object value)
        {
            List<DataCell> cells = new();

            for (int _ = 0; _ < Columns.Count; _++)
                cells.AddRange(Columns[_].FindValues(value));

            return cells;
        }
        public IEnumerable<DataCell> GetAllFromColumn(int ColumnIndex)
        {
            if (!ColumnIndex.IsBetween(0, Headers.Length))
                throw new IndexOutOfRangeException("The column index is out of range");

            return Rows.Select(row => row.Cells[ColumnIndex]);
        }



        // IO
        private void ParseCsvString(string CsvAsString, bool FirstLineIsHeader, Func<string, string> ValueFilter)
        {
            string[] lines = CsvAsString.Split('\n');
            Columns = new List<DataColumn>();
            var headers = lines[0].Split(',');

            if (!FirstLineIsHeader)
                headers = Enumerable.Range(0, headers.Length).Select(x => x.ToString()).ToArray();
            else
                for (int i = 0; i < headers.Length; i++)
                    Columns.Add(new DataColumn(this, headers[i], i));

            Rows = new List<DataRow>();

            int StartIndex = FirstLineIsHeader ? 1 : 0;
            int count = 0;



            for (int i = StartIndex; i < lines.Length - 1; i++)
            {

                string[] values = lines[i].Split(',');
                if (values.Length != headers.Length)
                    continue;

                if (ValueFilter is not null)
                {
                    values = lines[i].Split(',');
                    for (int j = 0; j < values.Length; j++)
                        values[j] = ValueFilter(values[j]); // Apply the filter to the value before adding it to the array
                }


                Rows.Add(new DataRow(values, count, this));
                count++;
            }

            _CsvLoaded = true;
        }
        public void ReadFromFile(string path, bool FirstLineIsHeader = true, Func<string, string> ValueFilter = null)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("The file does not exist");

            ParseCsvString(File.ReadAllText(path), FirstLineIsHeader, ValueFilter);
            _CsvPath = path;
            _Source = CsvSource.File;
        }

        // Web
        public async Task ReadFromWeb(string Url, bool FirstLineIsHeader = true, Func<string, string> ValueFilter = null)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(Url);
                ParseCsvString(response, FirstLineIsHeader, ValueFilter);
                _CsvLoaded = true;
                _Source = CsvSource.Web;
            }

        }


        // IO
        public string SerializeAsCsv()
        {
            if (!_CsvLoaded)
                throw new InvalidOperationException("The table has not been loaded from a file yet.");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Join(",", Headers));

            foreach (DataRow row in Rows)
                sb.AppendLine(string.Join(",", row.Cells.Select(cell => cell.Value)));

            return sb.ToString();
        }
        public void SaveChangesAndOveride()
        {
            if (_Source == CsvSource.Web)
                throw new InvalidOperationException("The table has been loaded from a web source, you can't save changes to it. use SaveChanges(str) instead.");

            SaveChanges(_CsvPath);
        }
        public void SaveChanges(string path)
        {
            switch (_Source)
            {
                case CsvSource.NotSet:
                    throw new InvalidOperationException("The table has not been loaded from a file yet.");
            }

            File.WriteAllText(path, SerializeAsCsv());
            _CsvPath = path;
        }
        public void SaveLogs(string path)
        {
            if (!CaptureLogs)
                throw new InvalidOperationException("The logs are not being captured");

            File.WriteAllLines(path, ChangeLogs);
        }
        // Output
        public void PrintTable(int PrintAmount = -1)
        {
            Console.WriteLine(string.Join(" | ", Headers));

            int TotalRows = PrintAmount == -1 || PrintAmount > Rows.Count ? Rows.Count : PrintAmount;

            for (int i = 0; i < TotalRows; i++)
                Console.WriteLine(Rows[i]);
        }


        // Experssions
        public DataCell GetCellAt(CellPosition position) => GetCellAt(position.X, position.Y);
        public DataCell GetCellAt(int x, int y) => Rows[x].Cells[y];
        public void ReadFromFile(string path, Func<string, string> ValueFilter) => ReadFromFile(path, true, ValueFilter);
        public void ReadFromFile(string path, bool FirstLineIsHeader) => ReadFromFile(path, FirstLineIsHeader, null);
        internal int GetHeaderIndex(string header) => Array.IndexOf(Headers, header);
    }
}
