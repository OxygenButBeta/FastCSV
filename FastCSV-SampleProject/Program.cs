
// include o2.CSVReader namespace to use the library
using o2.FastCSV;


/**************************************************************/
// this sample csv data belongs to the Florida State University
// for more information about the license of the data : https://people.sc.fsu.edu/~jburkardt/txt/gnu_lgpl.txt
const string WebCsvUrl = "https://people.sc.fsu.edu/~jburkardt/data/csv/oscar_age_male.csv";






// Read the CSV file from the web and remove quotes from values
// The second parameter is a boolean that indicates if the first row is for headers
// The third parameter is a function that will be applied to each cell value 
// In this case, we are replacing the quotes with a space
DataTable CsvTable = await CsvReader.ReadFromWeb(WebCsvUrl, true, (str) => str.Replace('"', ' '));

/*  You can also read from a file
         DataTable csv = CsvReader.ReadAllFromFile(@"G:\customers-100000.csv"); or 
            DataTable csv = CsvReader.ReadAllFromFile(@"G:\customers-100000.csv", true); or
            DataTable csv = CsvReader.ReadAllFromFile(@"G:\customers-100000.csv", true, (str) => str.Replace('"', ' '));
*/


// CaptureChanges parameter is optional and default value is false
// if you set the captureLogs to true, anytime anyvalue is changed in the table, it will be logged
// You can access the logs using the ChangeLogs property
CsvTable.CaptureChanges = true;


//Remove quotes from headers
foreach (var column in CsvTable.Columns)
    column.Header = item.Header.Replace('"', ' ').Trim();

// Print the table
// you can also give a int as a parameter to define the number of rows to print
CsvTable.PrintTable(); // or CsvTable.PrintTable(10); to print the first 10 rows


// Get the column "Age"
var Column = CsvTable["Age"];
/*
 * The column "Age" is a DataColumn object that contains all the cells in the column
 * You can get the cells using the GetCells() method
 * You can also get the column header using the Indexer 
    CsvTable.Columns[2];
 */

// Find the youngest actor in the data with using the column "Age" and LINQ
var YoungestActor = Column.GetCells().OrderBy(x => x.Value).FirstOrDefault();

// The c "YoungestActor" is a DataCell object that contains the value and the related row
// By default a ToString() method is implemented that returns the position, data type and value of the cell in the table,
Console.WriteLine($"\nCell ToString() : {YoungestActor}");

// You can get the position of the cell in the table using the Position property
CellPosition Position = YoungestActor.Position;

// You can get the value of the cell using the Value property
// Data type will be automatically converted to the correct type when you set the value of the cell
Type dataType = YoungestActor.CellDataType;
Console.WriteLine($"\nCell Data Type {dataType}");


// You can find the exact row of the cell using the RelatedRow property
Console.WriteLine($"\nYoungest Actor in the data : {YoungestActor?.RelatedRow}");

// You can also get the next row and the previous row if its exist
// if the row is the first row, the Previous() method will cause an exception
// if the row is the last row, the Next() method will cause an exception
Console.WriteLine($"\nNext Row : {YoungestActor?.RelatedRow.Next()}");
Console.WriteLine($"\nPrevious Row : {YoungestActor?.RelatedRow.Previous()}");

// You can get the value of the exact row using the Indexer
// Data entities ara related to each other
Console.WriteLine($"\nMovie {YoungestActor.RelatedRow["Movie"].Value}");


// List the names of the actors who won the Oscar before 2000

// Extract the column "Year"
var YearColumn = CsvTable["Year"];

// Get the cells that have a value less than 2000 using LINQ
var WinnersBefore2000 = YearColumn.GetCells().Where(x => (int)x.Value < 2000).ToList();

Console.WriteLine("\nList of the actors who won the Oscar before year 2000\n");
foreach (var item in WinnersBefore2000)
    Console.WriteLine(item.RelatedRow);

// Trim the names of the actors in the data
var Names = CsvTable["Name"].GetCells();
foreach (var name in Names)
    name.Value = name.Value.ToString().Trim();


// Lastly, Lets find the actor who won the Oscar more than once
// Extract the column "Name"
var ActorColumn = CsvTable["Name"];
var ActorCells = ActorColumn.GetCells();

// Group the actors by their names and get the actors who won more than once with LINQ

var ActorsWhoWonMoreThanOnceCells = ActorCells
       .GroupBy(c => c.Value)
            .Where(g => g.Count() > 1).
            Select(g => g)
            .SelectMany(g => g)
            .ToList();


Console.WriteLine("\n-- Actors who won oscar more than one --\n");
foreach (var actor in ActorsWhoWonMoreThanOnceCells)
{
    var sBuilder = new System.Text.StringBuilder();
    var relatedRow = actor.RelatedRow;

    sBuilder.Append(relatedRow["Name"].Value);
    sBuilder.Append("won the Oscar in ");
    sBuilder.Append(relatedRow["Year"].Value);
    sBuilder.Append(" With movie named");
    sBuilder.Append(relatedRow["Movie"].Value);

    Console.WriteLine(sBuilder);
}
// You can print the change logs if you want
Console.WriteLine("\n\n-Change Logs-\n\n");
foreach (var log in CsvTable.ChangeLogs)
    Console.WriteLine(log);

// if you desire, you can save the logs to a file
CsvTable.SaveLogs("logs.txt");

// Save the changes to the file
CsvTable.SaveToFile("newfile.csv");

// You can save the changes to a new file or override the source file
// But you can't save the changes to the web source so be sure to check the source before saving

/*
 * in this sample, the source is a web source
 * this will cause an exception
 * CsvTable.SaveChangesAndOveride();
*/

if (CsvTable._Source == DataTable.CsvSource.File)
    CsvTable.SaveChangesAndOverride();


// Wait for the user to press a key
Console.ReadLine();
