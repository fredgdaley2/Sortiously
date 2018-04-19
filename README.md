# Sortiously

#### Sortiously - Sort large and small delimited or fixed width files by defined key(s) and direction with optional data filtering and progress reporting.

*Additionally can merge purge files. Documentation in [**ReadMeMergePurge.txt**](Sortiously/ReadMeMergePurge.txt)

### Installing Sortiously ##

Use [NuGet](https://www.nuget.org) to install. Run the following command 
in the [Package Manager Console](http://docs.nuget.org/consume/package-manager-console).

```
PM> Install-Package Sortiously
```
##### *Dependencies:* System.Data.SQLite.Core
*Uses SQLite to do the sorting according to the custom defined key(s) and direction.*

* Very CPU and memory friendly.
* The original file is not overwritten or changed.
* Thread safe.

### Features

* Sort using multiple keys, direction and unique keys. [see Sort Definitions](#defining-sortdefinitions-for-sorting-a-delimited-file)
* Filter out data while reading file.
* Remove duplicate data using unique keys. Choose to have duplicates written out to file.
* Select either to write out the sorted file and/or receive the data through an Action method after sorting. [see DataTransportation](#datatransportation-parameter)
* Customize the keys from the data using delegates. [see Examples](#examples)

**Progress Reporting:** Use this knowing that it will slow down the process due to writing to console or other output source.  So if you want pure speed don't report the progress.

#### The progress report object:
[See Example](#example-showing-how-to-implement-the-progress-reporting)

```csharp
public class SortProgress
{
    //true when reading the file.
    public bool Reading { get; set; }
    //true when writing the file
    public bool Writing { get; set; }
    //the line number of the corresponding operation reading or writing;
    public int Counter { get; set; }
}
 ```

#### All functions return a SortResults instance.

```csharp
public class SortResults
{
    public int LinesRead { get; set; }
    public int LinesSorted { get; set; }
    public int Duplicates { get; set; }
    public int Filtered { get; set; }
    public string SortedFilePath { get; set; }
    public string DuplicatesFilePath { get; set; }
}
```

**LinesRead:** The number of lines read minus the header if included.

**LinesSorted:** The number of lines in the sorted file minus the header if included.

**Duplicates:** The number of duplicates when using a unique key.

**Filtered:** The number of filtered lines when using a filter.

**SortedFilePath:** The full path of the sorted file

**DuplicatesFilePath:** The full path of the file that contains the duplicates when using a unique key. Duplicates will include the
first unique key and data and the subsequent duplicates will be put in the duplicates file. If the source file has an header the
duplicates file will also include the header.

**SortedFilePath:** The original sorted file name and extension suffixed with _sorted i.e. FileToBeSorted_sorted.tsv

**DuplicatesFilePath:** The original sorted file name and extension suffixed with _dupes i.e. FileToBeSorted_dupes.tsv

#### Self cleanup:
Anytime during the process when an exception is encountered any files produced will be removed.  The exception will then be thrown so the consuming application can take the appropriate action.


#### There are five methods each to sort a delimited or fixed width file. Each method has an optional data filter and progress reporting.

1. Sort by defining SortDefinitions.
2. Sort using a numeric key providing a column number for delimited files or starting position and length for fixed width files.
3. Sort using a alphanumeric key providing a column number and length for delimited files or starting position and length for fixed width files.
4. Sort using a numeric key but the key is defined using a delegate, anonymous method, to provide custom key construction.
5. Sort using a alphanumeric key but the key is defined using a delegate, anonymous method, to provide custom key construction.

#### DataTransportation Parameter:
Each sort method can take an optional DataTransportation parameter.
Use the provided DataTransportationFactory methods to set the parameter.

```csharp
//This is the default it will write out the sorted file. (can be ommitted)
 dataTransportation: DataTransportationFactory.CreateAsFile()

//This will write out the file and pass each line through the Action method. 
 dataTransportation: DataTransportationFactory.CreateAsFileAndPassthrough((sortedData) =>
{
    string doSomethingWithData = sortedData;
})

//or
 dataTransportation: DataTransportationFactory.CreateAsFileAndPassthrough(PassMeTheData)

void PassMeTheData(string sortedData) 
{
   string doSomethingWithData = sortedData;
}

//Just passes each line through the Action method. The sorted file will not be written out. 
 dataTransportation: DataTransportationFactory.CreateAsPassthrough((sortedData) =>
{
    string doSomethingWithData = sortedData;
})

```


#### Here are the methods.

##### Sorts a delimited file by defining SortDefinitions.    
[See Example](#defining-sortdefinitions-for-sorting-a-delimited-file)

```csharp
public static SortResults SortDelimitedByKeyDefinitions(
                           string sourcefilePath,
                           SortDefinitions sortDefinitions,
                           Action<string[], string, string[]> setKeys,
                           Func<string[], string, bool> dataFilter = null,
                           DataTransportation dataTransportation = null,
                           string destinationFolder = null,
                           string delimiter = Constants.Delimiters.Comma,
                           bool hasHeader = true,
                           bool returnDuplicates = false,
                           Action<SortProgress> progress = null,
                           int maxBatchSize = 250000)
```

##### Sorts a fixed width file by defining SortDefinitions.
[See Example](#defining-sortdefinitions-for-sorting-a-fixed-width-file)

```csharp
public static SortResults SortFixedWidthByKeyDefinitions(string sourcefilePath,
                           SortDefinitions sortDefinitions,
                           Action<string, string[]> setKeys,
                           Func<string, bool> dataFilter = null,
                           DataTransportation dataTransportation = null,
                           string destinationFolder = null,
                           bool hasHeader = true,
                           bool returnDuplicates = false,
                           Action<SortProgress> progress = null,
                           int maxBatchSize = 250000)
```
##### Sorts a delimited file given a numeric key.

```csharp
public static SortResults SortDelimitedByNumericKey(
                               string sourcefilePath,
                               Func<string[], string, bool> dataFilter = null,
                               DataTransportation dataTransportation = null,
                               string destinationFolder = null,
                               string delimiter = Constants.Delimiters.Comma,
                               bool hasHeader = true,
                               int keyColumn = 0,
                               bool isUniqueKey = false,
                               bool returnDuplicates = false,
                               SortDirection sortDir = SortDirection.Ascending,
                               Action<SortProgress> progress = null,
                               int maxBatchSize = 250000)
```

##### Sorts a delimited file given a alphanumeric key.

```csharp
public static SortResults SortDelimitedByAlphaNumKey(
                               string sourcefilePath,
                               Func<string[], string, bool> dataFilter = null,
                               DataTransportation dataTransportation = null,
                               string destinationFolder = null,
                               string delimiter = Constants.Delimiters.Comma,
                               bool hasHeader = true,
                               int keyColumn = 0,
                               int keyLength = 15,
                               bool isUniqueKey = false,
                               bool returnDuplicates = false,
                               SortDirection sortDir = SortDirection.Ascending,
                               Action<SortProgress> progress = null,
                               int maxBatchSize = 250000)
```

##### Sorts a delimited file given a numeric key.

```csharp
public static SortResults SortDelimitedByNumericKey(
                               string sourcefilePath,
                               Func<string[], string, long> getKey,
                               Func<string[], string, bool> dataFilter = null,
                               DataTransportation dataTransportation = null,
                               string destinationFolder = null,
                               string delimiter = Constants.Delimiters.Comma,
                               bool hasHeader = true,
                               bool isUniqueKey = false,
                               bool returnDuplicates = false,
                               SortDirection sortDir = SortDirection.Ascending,
                               Action<SortProgress> progress = null,
                               int maxBatchSize = 250000)
```

##### Sorts a delimited file given a alphanumeric key.

```csharp
public static SortResults SortDelimitedByAlphaNumKey(
                               string sourcefilePath,
                               Func<string[], string, string> getKey,
                               Func<string[], string, bool> dataFilter = null,
                               DataTransportation dataTransportation = null,
                               string destinationFolder = null,
                               string delimiter = Constants.Delimiters.Comma,
                               bool hasHeader = true,
                               bool isUniqueKey = false,
                               bool returnDuplicates = false,
                               SortDirection sortDir = SortDirection.Ascending,
                               Action<SortProgress> progress = null,
                               int maxBatchSize = 250000)
```

##### Sorts a fixed width file given a numeric key.

```csharp
 public static SortResults SortFixedWidthByNumericKey(
                               string sourcefilePath,
                               Func<string, bool> dataFilter = null,
                               DataTransportation dataTransportation = null,
                               string destinationFolder = null,
                               bool hasHeader = true,
                               FixedWidthKey keyDef = null,
                               bool isUniqueKey = false,
                               bool returnDuplicates = false,
                               SortDirection sortDir = SortDirection.Ascending,
                               Action<SortProgress> progress = null,
                               int maxBatchSize = 250000)
```

##### Sorts a fixed width file given a alphanumeric key.

```csharp
public static SortResults SortFixedWidthByAlphaNumKey(
                               string sourcefilePath,
                               Func<string, bool> dataFilter = null,
                               DataTransportation dataTransportation = null,
                               string destinationFolder = null,
                               bool hasHeader = true,
                               FixedWidthKey keyDef = null,
                               bool isUniqueKey = false,
                               bool returnDuplicates = false,
                               SortDirection sortDir = SortDirection.Ascending,
                               Action<SortProgress> progress = null,
                               int maxBatchSize = 250000)
```

##### Sorts a fixed width file given a numeric key.

```csharp
public static SortResults SortFixedWidthByNumericKey(
                               string sourcefilePath,
                               Func<string, long> getKey,
                               Func<string, bool> dataFilter = null,
                               DataTransportation dataTransportation = null,
                               string destinationFolder = null,
                               bool hasHeader = true,
                               bool isUniqueKey = false,
                               bool returnDuplicates = false,
                               SortDirection sortDir = SortDirection.Ascending,
                               Action<SortProgress> progress = null,
                               int maxBatchSize = 250000)
```

##### Sorts a fixed width file given a alphanumeric key.

```csharp
public static SortResults SortFixedWidthByAlphaNumKey(
                               string sourcefilePath,
                               Func<string, string> getKey,
                               Func<string, bool> dataFilter = null,
                               DataTransportation dataTransportation = null,
                               string destinationFolder = null,
                               bool hasHeader = true,
                               bool isUniqueKey = false,
                               bool returnDuplicates = false,
                               SortDirection sortDir = SortDirection.Ascending,
                               Action<SortProgress> progress = null,
                               int maxBatchSize = 250000)
```

#### Examples:

#### Defining SortDefinitions for sorting a delimited file.
_see MockData.csv_

```csharp
static void SortDelimitedByKeyDefinitions()
{
    SortDefinitions sortDefs = new SortDefinitions();
    sortDefs.Add(new SortDefinition { DataType = KeyType.AlphaNumeric, Direction = SortDirection.Ascending });
    sortDefs.Add(new SortDefinition { DataType = KeyType.Numberic, Direction = SortDirection.Descending });

    SortResults srtResults = SortFile.SortDelimitedByKeyDefinitions(
        sourcefilePath: Path.Combine(masterSourceFolder, "MockData.csv"),
        sortDefinitions: sortDefs,
        setKeys: (fields, line, keyValues) =>
        {
            //gender
            keyValues[0] = fields[4];
            //DateReceived
            keyValues[1] = SortHelpers.JulianDateForSort(Convert.ToDateTime(fields[7])).ToString();
        },
        destinationFolder: masterDestFolder,
        delimiter: Constants.Delimiters.Comma,
        progress: ReportProgress);
}

```

#### Defining SortDefinitions for sorting a fixed width file.
_see FWMockDataDetail.txt_

```csharp
static void SortFixedWithByKeyDefinitions()
{
    SortDefinitions sortDefs = new SortDefinitions();
    sortDefs.Add(new SortDefinition { DataType = KeyType.Numberic, Direction = SortDirection.Descending });
    sortDefs.Add(new SortDefinition { DataType = KeyType.AlphaNumeric, Direction = SortDirection.Ascending, IsUniqueKey = true });
    sortDefs.Add(new SortDefinition { DataType = KeyType.AlphaNumeric, Direction = SortDirection.Ascending, IsUniqueKey = true });

    SortResults srtResults = SortFile.SortFixedWidthByKeyDefinitions(
        sourcefilePath: Path.Combine(masterSourceFolder, "FWMockDataDetail.txt"),
        sortDefinitions: sortDefs,
        setKeys: SetFWKeys,
        hasHeader: false,
        destinationFolder: masterDestFolder,
        progress: ReportProgress);
}

static void SetFWKeys(string line, string[] keyValues)
{
    FileParser.ParseFixedWidthString(new StringReader(line), (fields, lineNum) =>
    {
        keyValues[0] = SortHelpers.JulianDateForSort(Convert.ToDateTime(fields[7])).ToString();
        keyValues[1] = fields[4];
        keyValues[2] = fields[6].PadKeyWithZero(20);
    }, new int[] { 5, 9, 10, 32, 7, 15, 20, 10 });
}
```
#### Basic usage

```csharp
SortResults srtResults = SortFile.SortDelimitedByAlphaNumKey(
        sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMe.tsv",
        destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
        delimiter: Delimiters.Tab,
        keyColumn: 2,
        keyLength: 11,
        isUniqueKey: true,
        sortDir: SortDirection.Descending);
```
#### Defining the maxBatchSize for the batch insertion _default is 250000_
_May need to adjust when encountering an out of memory exception._

```csharp
SortResults srtResults = SortFile.SortDelimitedByAlphaNumKey(
        sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMe.tsv",
        destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
        delimiter: Delimiters.Tab,
        keyColumn: 2,
        keyLength: 11,
        isUniqueKey: true,
        sortDir: SortDirection.Descending,
        maxBatchSize: 100000);
```

#### Example showing how to implement the progress reporting.

```csharp
SortResults srtResults = SortFile.SortDelimitedByAlphaNumKey(
    sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMe.tsv",
    destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
    delimiter: Delimiters.Tab,
    keyColumn: 2,
    keyLength: 11,
    isUniqueKey: true,
    sortDir: SortDirection.Descending,
    progress: ReportProgress);

static void ReportProgress(SortProgress srtProgress)
{
    if (srtProgress.Reading)
    {
        Console.Write("\rReading : " + srtProgress.Counter.ToString().PadRight(30,' '));
    }
    else
    {
        Console.Write("\rWriting : " + srtProgress.Counter.ToString().PadRight(30, ' '));
    }
}
```


#### Example of custom key construction with a lambda expression

```csharp
SortResults srtResults = SortFile.SortDelimitedByAlphaNumKey(
        sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMe.tsv",
        getKey: (fields, line) => fields[2].Trim().PadLeft(11, '0'),
        destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
        delimiter: Delimiters.Tab,
        isUniqueKey: true,
        sortDir: SortDirection.Ascending);



SortResults srtResults = SortFile.SortFixedWidthByAlphaNumKey(
         sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMeFW.txt",
         destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
         hasHeader: false,
         keyDef: new FixedWidthKey { StartPos = 0, KeyLength = 12 },
         isUniqueKey: false,
         sortDir: SortDirection.Ascending);
```

#### Example of custom key construction

```csharp
SortResults srtResults = SortFile.SortFixedWidthByAlphaNumKey(
         sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMeFW.txt",
         getKey: GetFixedKey,
         destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
         hasHeader: false,
         isUniqueKey: false,
         sortDir: SortDirection.Ascending);

 static string GetFixedKey(string line)
 {
     return line.Substring(0, 12).Trim().PadLeft(12, '0');
 }
```


#### Example of custom key construction and data filtering

```csharp
 static void SortDelimitedGetKeyTestCsv()
 {
     SortResults srtResults = SortFile.SortDelimitedByAlphaNumKey(
         sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortIt.csv",
         getKey: GetNameKey,
         dataFilter: FilterOutBlankNames,
         destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
         delimiter: Delimiters.Comma,
         isUniqueKey: false,
         sortDir: SortDirection.Ascending);

     Console.WriteLine("Lines Read  : " + srtResults.LinesRead.ToString());
     Console.WriteLine("Lines Sorted: " + srtResults.LinesSorted.ToString());
 }

 static string GetNameKey(string[] fields, string line)
 {
    //fields[7] = last name, fields[6] = first name
    string key = string.Join(" ",fields[7].Trim().ToLower(),fields[6].Trim().ToLower());
    return key;
 }

 static bool FilterOutBlankNames(string[] fields, string line)
 {
     string key = fields[7].Trim() + fields[6].Trim();
     if (string.IsNullOrWhiteSpace(key))
     {
         //will be be filtered out
         return false;
     }
     //will not be filtered out
     return true;
 }
```

#### Miscellaneous

Import Sortiously.Framework namespace to gain access to the delimiters.

```csharp
namespace Sortiously.Framework
{

    public static partial class Constants
    {
        public static class Delimiters
        {
            public const string Comma = ",";
            public const string Tab = "\t";
            public const string Pipe = "|";
            public const string SemiColon = ";";
            public const string Caret = "^";
        }
    }
}
```
Import Sortiously.StringExtns namespace to gain access to some helper extensions

```csharp
 public static string LeftAlign(this string value, int totWidth)
 {
     return value.PadRight(totWidth);
 }

 public static string RightAlign(this string value, int totWidth)
 {
     return value.PadLeft(totWidth);
 }

 public static string TruncateFixed(this string value, int totWidth)
 {
     if (value.Length > totWidth)
     {
         value = value.Substring(0, totWidth);
     }
     return value;
 }

 public static string PadKeyWithZero(this string value, int keyLength)
 {
     return value.Trim().PadLeft(keyLength, '0');
 }

 public static string PadKeyWithA(this string value, int keyLength)
 {
     return value.Trim().PadRight(keyLength, 'A');
 }

```

*Additionally there are file parsing methods in the **FileParsers.cs** class.*


MIT License.