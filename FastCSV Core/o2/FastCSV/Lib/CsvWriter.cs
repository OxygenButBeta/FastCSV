using System.Text;
using static o2.FastCSV.DataTable;

namespace o2.FastCSV
{
    public static class CsvWriter
    {
        public static string SerializeTable(this DataTable table)
        {
            if (!table._CsvLoaded)
                throw new InvalidOperationException("The table has not been loaded from a file yet.");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Join(",", table.Headers));

            foreach (DataRow row in table.Rows)
                sb.AppendLine(string.Join(",", row.Cells.Select(cell => cell.Value)));

            return sb.ToString();
        }
        public static void SaveToFile(this DataTable table, string path)
        {
            switch (table._Source)
            {
                case CsvSource.NotSet:
                    throw new InvalidOperationException("The table has not been loaded yet.");
            }

            File.WriteAllText(path, table.SerializeTable());
            table._CsvPath = path;
        }
        public static async Task SaveToFileAsync(this DataTable table, string path)
        {
            switch (table._Source)
            {
                case CsvSource.NotSet:
                    throw new InvalidOperationException("The table has not been loaded yet.");
            }

            await File.WriteAllTextAsync(path, table.SerializeTable());
            table._CsvPath = path;
        }
    }
}
