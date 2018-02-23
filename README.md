# Sortiously

    Sortiously - Sort a delimited or fixed width files by a defined key with options filter data and report progress.

    Dependencies: System.Data.SQLite.Core

    Sort more than one file at a time using background workers or multiple threads.

    Progress Reporting: Use this knowing that it will slow down the process due to writing to console or other output source.
    In testing when doing a file with 175,000 plus lines it took about 25 to 30 seconds more to finish the process. So if you want pure
    speed don't report the progress.

    The progress report object, SortProgress looks like this:
    
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

    The original file is not overwritten or changed.
    All functions return a SortResults instance.

    public class SortResults
    {
        public int LinesRead { get; set; }
        public int LinesSorted { get; set; }
        public int Duplicates { get; set; }
        public int Filtered { get; set; }
        public string SortedFilePath { get; set; }
        public string DuplicatesFilePath { get; set; }
    }

    LinesRead: The number of lines read minus the header if included.
    LinesSorted: The number of lines in the sorted file minus the header if included.
    Duplicates: The number of duplicates when using a unique key.
    Filtered: The number of filtered lines when using a filter.
    SortedFilePath: The full path of the sorted file.
    DuplicatesFilePath: The full path of the file that contains the duplicates when using a unique key. Duplicates will include the
    first unique key and data and the subsequent duplicates will be put in the duplicates file. If the source file has an header the
    duplicates file will also include the header.


    SortedFilePath: The original sorted file name and extension suffixed with _ds_sorted i.e. FileToBeSorted_ds_sorted.tsv
    DuplicatesFilePath: The original sorted file name and extension suffixed with _ds_dupes i.e. FileToBeSorted_ds_dupes.tsv


    Self cleanup: Anytime during the process when an exception is encountered any files produced will be removed.  The exception will then be thrown
    so the consuming application can take the appropriate action.


    There are four methods each to sort a delimited or fixed width file. Each method has an options filter data and report progress.

    1. Sort using a numeric key providing a column number for delimited files or starting position and length for fixed width files.
    2. Sort using a alphanumeric key providing a column number and length for delimited files or starting position and length for fixed width files.
    3. Sort using a numeric key but the key is defined using a delegate, anonymous method, to provide custom key construction.
    4. Sort using a alphanumeric key but the key is defined using a delegate, anonymous method, to provide custom key construction.


    Very CPU and memory friendly.

    Here are the methods.

        /// <summary>
        /// Sorts a delimited file given a numeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="delimiter">Character delimiter</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="keyColumn">The zero based column number to be used as the key to sort</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        public static SortResults SortDelimitedByNumericKey(string sourcefilePath,
                                   Func<string[], string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   string delimiter = Constants.Delimiters.Comma,
                                   bool hasHeader = true,
                                   int keyColumn = 0,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)



        /// <summary>
        /// Sorts a delimited file given a alphanumeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="delimiter">Character delimiter</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="keyColumn">The zero based column number to be used as the key to sort</param>
        /// <param name="keyLength">The length of the key right justified with zeros if less than length specified</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        public static SortResults SortDelimitedByAlphaNumKey(string sourcefilePath,
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


        /// <summary>
        /// Sorts a delimited file given a numeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="getKey">Function to construct the key</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="delimiter">Character delimiter</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
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

        /// <summary>
        /// Sorts a delimited file given a alphanumeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="getKey">Function to construct the key</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="delimiter">Character delimiter</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        public static SortResults SortDelimitedByAlphaNumKey(string sourcefilePath,
                                   Func<string[], string, string> getKey,
                                   Func<string[], string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   string delimiter = Constants.Delimiters.Comma,
                                   bool hasHeader = true,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)


        /// <summary>
        /// Sorts a fixed width file given a numeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="keyDef">The zero based starting position of the key and length, will be trimmed.</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        public static SortResults SortFixedWidthByNumericKey(string sourcefilePath,
                                   Func<string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   bool hasHeader = true,
                                   FixedWidthKey keyDef = null,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)

        /// <summary>
        /// Sorts a fixed width file given a alphanumeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="keyDef">The zero based starting position of the key and length, will be trimmed.</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        public static SortResults SortFixedWidthByAlphaNumKey(string sourcefilePath,
                                   Func<string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   bool hasHeader = true,
                                   FixedWidthKey keyDef = null,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)

        /// <summary>
        /// Sorts a fixed width file given a numeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="getKey">Function to construct the key</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        public static SortResults SortFixedWidthByNumericKey(string sourcefilePath,
                                   Func<string, long> getKey,
                                   Func<string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   bool hasHeader = true,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)

        /// <summary>
        /// Sorts a fixed width file given a alphanumeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="getKey">Function to construct the key</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        public static SortResults SortFixedWidthByAlphaNumKey(string sourcefilePath,
                                   Func<string, string> getKey,
                                   Func<string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   bool hasHeader = true,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null)


        Examples:

        SortResults srtResults = SortFile.SortDelimitedByAlphaNumKey(sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMe.tsv",
                destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
                delimiter: Delimiters.Tab,
                keyColumn: 2,
                keyLength: 11,
                isUniqueKey: true,
                sortDir: SortDirection.Descending);



        Example showing how to implement the progress reporting.

            SortResults srtResults = SortFile.SortDelimitedByAlphaNumKey(sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMe.tsv",
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


        Example of custom key construction with a lambda expression

        SortResults srtResults = SortFile.SortDelimitedByAlphaNumKey(sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMe.tsv",
                getKey: (fields, line) => fields[2].Trim().PadLeft(11, '0'),
                destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
                delimiter: Delimiters.Tab,
                isUniqueKey: true,
                sortDir: SortDirection.Ascending);



       SortResults srtResults = SortFile.SortFixedWidthByAlphaNumKey(sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMeFW.txt",
                destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
                hasHeader: false,
                keyDef: new FixedWidthKey { StartPos = 0, KeyLength = 12 },
                isUniqueKey: false,
                sortDir: SortDirection.Ascending);


       Example of custom key construction

       SortResults srtResults = SortFile.SortFixedWidthByAlphaNumKey(sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortMeFW.txt",
                getKey: GetFixedKey,
                destinationFolder: @"C:\TempGarbage\FilesToProcess\sorted",
                hasHeader: false,
                isUniqueKey: false,
                sortDir: SortDirection.Ascending);

        static string GetFixedKey(string line)
        {
            return line.Substring(0, 12).Trim().PadLeft(12, '0');
        }



        Example of custom key construction and data filtering

        static void SortDelimitedGetKeyTestCsv()
        {
            SortResults srtResults = SortFile.SortDelimitedByAlphaNumKey(sourcefilePath: @"C:\TempGarbage\FilesToProcess\SortIt.csv",
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
           string key = fields[7].Trim().ToLower() + " " + fields[6].Trim().ToLower();
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
