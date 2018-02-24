# Sortiously

#### Sortiously - Sort a delimited or fixed width files by a defined key with data filter options and report progress.

*Additionally can merge purge files. Documentation in **ReadMeMergePurge.txt***

##### *Dependencies:* System.Data.SQLite.Core

* Sort more than one file at a time using multiple threads.
* The original file is not overwritten or changed.
* Very CPU and memory friendly.

**Progress Reporting:** Use this knowing that it will slow down the process due to writing to console or other output source.  So if you want pure speed don't report the progress.

#### The progress report object:

    public class SortProgress
    {
        //true when reading the file.
        public bool Reading { get; set; }
        //true when writing the file
        public bool Writing { get; set; }
        //the line number of the corresponding operation reading or writing;
        public int Counter { get; set; }
    }

#### All functions return a SortResults instance.

    public class SortResults
    {
        public int LinesRead { get; set; }
        public int LinesSorted { get; set; }
        public int Duplicates { get; set; }
        public int Filtered { get; set; }
        public string SortedFilePath { get; set; }
        public string DuplicatesFilePath { get; set; }
    }

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


#### There are four methods each to sort a delimited or fixed width file. Each method has an options filter data and report progress.

1. Sort using a numeric key providing a column number for delimited files or starting position and length for fixed width files.
2. Sort using a alphanumeric key providing a column number and length for delimited files or starting position and length for fixed width files.
3. Sort using a numeric key but the key is defined using a delegate, anonymous method, to provide custom key construction.
4. Sort using a alphanumeric key but the key is defined using a delegate, anonymous method, to provide custom key construction.

#### Here are the methods.

##### Sorts a delimited file given a numeric key.

    public static SortResults SortDelimitedByNumericKey(
                                   string sourcefilePath,
                                   Func<string[], string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   string delimiter = Constants.Delimiters.Comma,
                                   bool hasHeader = true,
                                   int keyColumn = 0,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)



##### Sorts a delimited file given a alphanumeric key.

    public static SortResults SortDelimitedByAlphaNumKey(
                                   string sourcefilePath,
                                   Func<string[], string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   string delimiter = Constants.Delimiters.Comma,
                                   bool hasHeader = true,
                                   int keyColumn = 0,
                                   int keyLength = 15,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)

##### Sorts a delimited file given a numeric key.

    public static SortResults SortDelimitedByNumericKey(
                                   string sourcefilePath,
                                   Func<string[], string, long> getKey,
                                   Func<string[], string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   string delimiter = Constants.Delimiters.Comma,
                                   bool hasHeader = true,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)

##### Sorts a delimited file given a alphanumeric key.

    public static SortResults SortDelimitedByAlphaNumKey(
                                   string sourcefilePath,
                                   Func<string[], string, string> getKey,
                                   Func<string[], string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   string delimiter = Constants.Delimiters.Comma,
                                   bool hasHeader = true,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)


##### Sorts a fixed width file given a numeric key.

    public static SortResults SortFixedWidthByNumericKey(
                                   string sourcefilePath,
                                   Func<string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   bool hasHeader = true,
                                   FixedWidthKey keyDef = null,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)

##### Sorts a fixed width file given a alphanumeric key.

    public static SortResults SortFixedWidthByAlphaNumKey(
                                   string sourcefilePath,
                                   Func<string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   bool hasHeader = true,
                                   FixedWidthKey keyDef = null,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)

##### Sorts a fixed width file given a numeric key.

    public static SortResults SortFixedWidthByNumericKey(
                                   string sourcefilePath,
                                   Func<string, long> getKey,
                                   Func<string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   bool hasHeader = true,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)


##### Sorts a fixed width file given a alphanumeric key.

    public static SortResults SortFixedWidthByAlphaNumKey(
                                   string sourcefilePath,
                                   Func<string, string> getKey,
                                   Func<string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   bool hasHeader = true,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)

#### Examples:

        SortResults srtResults = SortFile.SortDelimitedByAlphaNumKey(
                sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMe.tsv",
                destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
                delimiter: Delimiters.Tab,
                keyColumn: 2,
                keyLength: 11,
                isUniqueKey: true,
                sortDir: SortDirection.Descending);


#### Example showing how to implement the progress reporting.

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


#### Example of custom key construction with a lambda expression

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


#### Example of custom key construction

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



#### Example of custom key construction and data filtering

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
                return false;
            }
            return true;
        }


MIT License.
