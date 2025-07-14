# Fast CSV

FastCSV is a lightweight and easy-to-use C# library for reading, processing, and saving CSV files. It supports loading CSV data from both local files and HTTP sources, and integrates seamlessly with LINQ for powerful data manipulation.

---

## Features

- 📂 **Read CSV Files** from local storage or web URLs.
- 🔍 **Process CSV Data** with LINQ queries for filtering, grouping, and more.
- 💾 **Save CSV Files** to new or existing files.
- 📝 **Capture and Log Changes** to track modifications to the data.
- 🗂️ **Flexible Data Handling** to easily access and manipulate columns and rows.

---

## Installation

- Add the `FastCSV.dll` to your project references.
- Import the namespace: `using o2.FastCSV;`

---

## Usage Example

The example below demonstrates reading CSV data from a web source, manipulating it, querying with LINQ, logging changes, and saving the results.

```csharp
using o2.FastCSV;
using System;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    // Sample CSV URL from Florida State University (license info: https://people.sc.fsu.edu/~jburkardt/txt/gnu_lgpl.txt)
    const string WebCsvUrl = "https://people.sc.fsu.edu/~jburkardt/data/csv/oscar_age_male.csv";

    static async Task Main()
    {
        // Read CSV from web, treat first row as headers, remove quotes from values
        DataTable CsvTable = await CsvReader.ReadFromWeb(WebCsvUrl, true, str => str.Replace('"', ' '));

        // Enable change capturing to log modifications
        CsvTable.CaptureChanges = true;

        // Remove quotes and trim headers
        foreach (var column in CsvTable.Columns)
            column.Header = column.Header.Replace('"', ' ').Trim();

        // Print the entire table or specify a row count, e.g. CsvTable.PrintTable(10);
        CsvTable.PrintTable();

        // Access the "Age" column
        var ageColumn = CsvTable["Age"];

        // Find youngest actor using LINQ
        var youngestActor = ageColumn.GetCells().OrderBy(x => x.Value).FirstOrDefault();

        Console.WriteLine($"\nYoungest Actor Cell Info: {youngestActor}");
        Console.WriteLine($"Cell Data Type: {youngestActor.CellDataType}");
        Console.WriteLine($"Position in Table: {youngestActor.Position}");
        Console.WriteLine($"Related Row Data: {youngestActor?.RelatedRow}");
        Console.WriteLine($"Next Row: {youngestActor?.RelatedRow.Next()}");
        Console.WriteLine($"Previous Row: {youngestActor?.RelatedRow.Previous()}");
        Console.WriteLine($"Movie: {youngestActor.RelatedRow["Movie"].Value}");

        // List actors who won Oscar before year 2000
        var yearColumn = CsvTable["Year"];
        var winnersBefore2000 = yearColumn.GetCells().Where(x => (int)x.Value < 2000).ToList();

        Console.WriteLine("\nActors who won the Oscar before 2000:");
        foreach (var winner in winnersBefore2000)
            Console.WriteLine(winner.RelatedRow);

        // Trim names in "Name" column
        var names = CsvTable["Name"].GetCells();
        foreach (var name in names)
            name.Value = name.Value.ToString().Trim();

        // Find actors who won Oscar more than once
        var actorColumn = CsvTable["Name"];
        var actorCells = actorColumn.GetCells();

        var multipleWinners = actorCells
            .GroupBy(c => c.Value)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .ToList();

        Console.WriteLine("\nActors who won Oscar more than once:");
        foreach (var actor in multipleWinners)
        {
            var row = actor.RelatedRow;
            Console.WriteLine($"{row["Name"].Value} won the Oscar in {row["Year"].Value} with movie \"{row["Movie"].Value}\"");
        }

        // Print change logs
        Console.WriteLine("\nChange Logs:");
        foreach (var log in CsvTable.ChangeLogs)
            Console.WriteLine(log);

        // Save logs to file
        CsvTable.SaveLogs("logs.txt");

        // Save changes to new file
        CsvTable.SaveToFile("newfile.csv");

        // Save changes and override source file if source is a file (not web)
        if (CsvTable._Source == DataTable.CsvSource.File)
            CsvTable.SaveChangesAndOverride();

        Console.ReadLine();
    }
}
