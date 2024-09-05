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
        internal string _CsvPath;
        internal bool _CsvLoaded;
        internal bool _CaptureChanges = false;


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
        public DataCell this[int x, int y] => this.GetCellAt(x, y);

        // Properties
        public CsvSource _Source { get; internal set; } = CsvSource.NotSet;
        public string[] Headers => Columns.Select(x => x.Header).ToArray();
        public List<DataRow> Rows { get; private set; }
        public List<DataColumn> Columns { get; private set; }
        public bool CheckCellEqualityAsString { get; set; } = true;
        public bool CaptureChanges
        {
            get => _CaptureChanges;
            set
            {
                _CaptureChanges = value;

                if (!_CaptureChanges)
                    ChangeLogs = null;
                else
                    ChangeLogs = new List<string>();

            }
        }
        public List<string> ChangeLogs { get; private set; }



        // IO
        internal void ParseCsvString(string lines, bool FirstLineIsHeader, Func<string, string> ValueFilter) => ParseCsvString(lines.Split('\n'), FirstLineIsHeader, ValueFilter);
        internal void ParseCsvString(string[] lines, bool FirstLineIsHeader, Func<string, string> ValueFilter)
        {
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
        public void SaveChangesAndOverride()
        {
            if (_Source == CsvSource.Web)
                throw new InvalidOperationException("The table has been loaded from a web source, you can't save changes to it. use SaveChanges(str) instead.");

            this.SaveToFile(_CsvPath);
        }
        public void SaveLogs(string path)
        {
            if (!CaptureChanges)
                throw new InvalidOperationException("The logs are not being captured");

            File.WriteAllLines(path, ChangeLogs);
        }

        // Experssions

        internal int GetHeaderIndex(string header) => Array.IndexOf(Headers, header);
    }
}
