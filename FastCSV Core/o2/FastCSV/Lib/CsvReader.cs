using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static o2.FastCSV.DataTable;

namespace o2.FastCSV
{
    public static class CsvReader
    {
        public static DataTable ReadAllFromFile(string path, bool FirstLineIsHeader = true, Func<string, string> ValueFilter = null)
        {
            var table = ValidatePathAndCreateTable(path);
            table.ParseCsvString(System.IO.File.ReadAllText(path), FirstLineIsHeader, ValueFilter);
            return table;
        }
        public static async Task<DataTable> ReadAllFromFileAsync(string path, bool FirstLineIsHeader = true, Func<string, string> ValueFilter = null)
        {
            var table = ValidatePathAndCreateTable(path);
            var data = await System.IO.File.ReadAllTextAsync(path);
            table.ParseCsvString(data, FirstLineIsHeader, ValueFilter);
            return table;

        }
        public static async Task<DataTable> ReadFromWeb(string Url, bool FirstLineIsHeader = true, Func<string, string> ValueFilter = null)
        {
            var table = new DataTable();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(Url);
                table.ParseCsvString(response, FirstLineIsHeader, ValueFilter);
                table._CsvLoaded = true;
                table._Source = CsvSource.Web;
            }
            return table;
        }
        public static DataTable ReadFromCsvData(string path, int DataLenght) => ReadFromCsvData(path, DataLenght, true, null);
        public static DataTable ReadFromCsvData(string path, int DataLenght, bool FirstLineAsHeaders, Func<string, string> ValueFilter)
        {
            var table = ValidatePathAndCreateTable(path);
            var lines = new string[DataLenght];
            using (var reader = new StreamReader(path))
            {
                DataLenght = FirstLineAsHeaders ? DataLenght + 1 : DataLenght;

                for (int i = 0; i < DataLenght; i++)
                    lines[i] = reader.ReadLine();
                reader.Close();
            }
            table.ParseCsvString(lines, FirstLineAsHeaders, ValueFilter);
            return table;
        }
        static DataTable ValidatePathAndCreateTable(string path)
        {
            if (!(path.EndsWith(".csv") && System.IO.File.Exists(path)))
                throw new ArgumentException("The path is not valid. " + path);
            return new DataTable();
        }
    }
}
