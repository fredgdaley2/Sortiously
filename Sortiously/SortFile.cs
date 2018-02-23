using System;
using System.IO;
using Sortiously.StringExtns;

namespace Sortiously
{
    public static class SortFile
    {
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
        {

            return SortDelimitedByKeyCore<long>(sourcefilePath: sourcefilePath,
                                             getKey: (fields, line) => long.Parse(fields[keyColumn]),
                                             dataFilter: dataFilter,
                                             destinationFolder: destinationFolder,
                                             delimiter: delimiter,
                                             hasHeader: hasHeader,
                                             isUniqueKey: isUniqueKey,
                                             returnDuplicates: returnDuplicates,
                                             sortDir: sortDir,
                                             progress: progress);
        }



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
        {
            return SortDelimitedByKeyCore<string>(sourcefilePath: sourcefilePath,
                                            getKey: (fields, line) => fields[keyColumn].PadKeyWithZero(keyLength),
                                            dataFilter: dataFilter,
                                            destinationFolder: destinationFolder,
                                            delimiter: delimiter,
                                            hasHeader: hasHeader,
                                            isUniqueKey: isUniqueKey,
                                            returnDuplicates: returnDuplicates,
                                            sortDir: sortDir,
                                            progress: progress);
        }

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
        {


            return SortDelimitedByKeyCore<long>(sourcefilePath: sourcefilePath,
                                             getKey: getKey,
                                             dataFilter: dataFilter,
                                             destinationFolder: destinationFolder,
                                             delimiter: delimiter,
                                             hasHeader: hasHeader,
                                             isUniqueKey: isUniqueKey,
                                             returnDuplicates: returnDuplicates,
                                             sortDir: sortDir,
                                             progress: progress);
        }


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
        {

            return SortDelimitedByKeyCore<string>(sourcefilePath: sourcefilePath,
                                            getKey: getKey,
                                            dataFilter: dataFilter,
                                            destinationFolder: destinationFolder,
                                            delimiter: delimiter,
                                            hasHeader: hasHeader,
                                            isUniqueKey: isUniqueKey,
                                            returnDuplicates: returnDuplicates,
                                            sortDir: sortDir,
                                            progress: progress);

        }


        /// <summary>
        /// Sorts a delimited file given a numeric or string key.
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
        internal static SortResults SortDelimitedByKeyCore<T>(
                                   string sourcefilePath,
                                   Func<string[], string, T> getKey,
                                   Func<string[], string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   string delimiter = Constants.Delimiters.Comma,
                                   bool hasHeader = true,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null,
                                   bool deleteDbConnPath = true,
                                   bool writeOutSortFile = true)
        {
            ArgumentValidation<T>(sourcefilePath, getKey,  delimiter,  destinationFolder);
            SortVars srtVars = new SortVars(sourcefilePath, destinationFolder);
            SortResults srtResults = new SortResults(sourcefilePath, srtVars.DestFolder, srtVars.DbConnPath);
            SortProgress srtProgress = new SortProgress();
            try
            {
                srtResults.DeleteDuplicatesFile();
                int lineCount = 1;
                using (StreamReader reader = new StreamReader(sourcefilePath))
                using (SqliteSortKeyBulkInserter<T> sortBulkInserter = new SqliteSortKeyBulkInserter<T>(srtVars.DbConnPath, uniqueKey: isUniqueKey))
                {
                    string line;
                    srtVars.Header = GetHeader(hasHeader, reader);
                    srtProgress.InitReading();
                    while ((line = reader.ReadLine()) != null)
                    {
                        srtResults.IncrementLinesRead();
                        ReportReadProgress(progress, srtProgress, srtResults.LinesRead);
                        FileParser.ParseDelimitedString(new StringReader(line), (fields, lNum) =>
                        {
                            if (dataFilter == null || dataFilter(fields, line))
                            {
                                T key = getKey(fields, line);
                                sortBulkInserter.Add(key, line + SortFileHelpers.EscapeByDelimiter(delimiter));
                                lineCount++;
                            }
                            else
                            {
                                srtResults.IncrementFiltered();
                            }
                        }, delimiter);
                    }
                    sortBulkInserter.InsertAnyLeftOvers();
                    sortBulkInserter.AddUnUniqueIndex();
                }
                srtProgress.InitWriting();

                if (writeOutSortFile)
                {
                    srtResults.WriteOutSorted(srtVars.DbConnPath, srtVars.Header, sortDir, delimiter, hasUniqueIndex: isUniqueKey, returnDuplicates: returnDuplicates,  dupesFilePath: srtResults.DuplicatesFilePath,  progress: (counter) => { srtProgress.Counter = counter; if (progress != null) { progress(srtProgress); } }, deleteDb: deleteDbConnPath);
                }
                else
                {
                    srtResults.Header = srtVars.Header;
                }


                srtResults.DeleteDuplicatesFileIfNoDuplicates();
            }
            catch (Exception)
            {
                CleanUp(srtVars, srtResults);
                srtProgress = null;
                throw;
            }
            return srtResults;
        }



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
        {
            return SortFixedWidthByKeyCore<long>(sourcefilePath: sourcefilePath,
                                   getKey: (line) => long.Parse(line.Substring(keyDef.StartPos, keyDef.KeyLength).Trim()),
                                   dataFilter: dataFilter,
                                   destinationFolder: destinationFolder,
                                   hasHeader: hasHeader,
                                   isUniqueKey: isUniqueKey,
                                   returnDuplicates: returnDuplicates,
                                   sortDir: sortDir,
                                   progress: progress);

        }

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
        {
            return SortFixedWidthByKeyCore<string>(sourcefilePath: sourcefilePath,
                                   getKey: (line) => line.Substring(keyDef.StartPos, keyDef.KeyLength).PadKeyWithZero(keyDef.KeyLength),
                                   dataFilter: dataFilter,
                                   destinationFolder: destinationFolder,
                                   hasHeader: hasHeader,
                                   isUniqueKey: isUniqueKey,
                                   returnDuplicates: returnDuplicates,
                                   sortDir: sortDir,
                                   progress: progress);

        }


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

        {
            return SortFixedWidthByKeyCore<long>(sourcefilePath: sourcefilePath,
                                   getKey: getKey,
                                   dataFilter: dataFilter,
                                   destinationFolder: destinationFolder,
                                   hasHeader: hasHeader,
                                   isUniqueKey: isUniqueKey,
                                   returnDuplicates: returnDuplicates,
                                   sortDir: sortDir,
                                   progress: progress);
        }


        /// <summary>
        /// Sorts a fixed width file given a numeric or string key.
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
        internal static SortResults SortFixedWidthByKeyCore<T>(string sourcefilePath,
                                   Func<string, T> getKey,
                                   Func<string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   bool hasHeader = true,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null,
                                   bool deleteDbConnPath = true,
                                   bool writeOutSortFile = true)

        {
            ArgumentValidation<T>(sourcefilePath, getKey, destinationFolder);
            SortVars srtVars = new SortVars(sourcefilePath, destinationFolder);
            SortResults srtResults = new SortResults(sourcefilePath, srtVars.DestFolder, srtVars.DbConnPath);
            SortProgress srtProgress = new SortProgress();
            try
            {
                srtResults.DeleteDuplicatesFile();
                int lineCount = 1;
                using (StreamReader reader = new StreamReader(sourcefilePath))
                using (SqliteSortKeyBulkInserter<T> sortBulkInserter = new SqliteSortKeyBulkInserter<T>(srtVars.DbConnPath, uniqueKey: isUniqueKey))

                {
                    string line;
                    srtVars.Header = GetHeader(hasHeader, reader);
                    srtProgress.InitReading();
                    while ((line = reader.ReadLine()) != null)
                    {
                        srtResults.IncrementLinesRead();
                        ReportReadProgress(progress, srtProgress, srtResults.LinesRead);
                        if (dataFilter == null || dataFilter(line))
                        {
                            T key = getKey(line);
                            sortBulkInserter.Add(key, (line + Constants.Common.PreserveCharacter).Compress());
                            lineCount++;
                        }
                        else
                        {
                            srtResults.IncrementFiltered();
                        }
                    }
                    sortBulkInserter.InsertAnyLeftOvers();
                    sortBulkInserter.AddUnUniqueIndex();
                }
                srtProgress.InitWriting();
                if (writeOutSortFile)
                {
                    srtResults.WriteOutSorted(dbConnPath: srtVars.DbConnPath, header: srtVars.Header, sortDir: sortDir, delimiter: Constants.Delimiters.Tab, hasUniqueIndex: isUniqueKey, returnDuplicates: returnDuplicates, dupesFilePath: srtResults.DuplicatesFilePath, compressed: true, progress: (counter) => { srtProgress.Counter = counter; if (progress != null) { progress(srtProgress); } }, deleteDb: deleteDbConnPath);
                }
                else
                {
                    srtResults.Header = srtVars.Header;
                }

                srtResults.DeleteDuplicatesFileIfNoDuplicates();
            }
            catch (Exception)
            {
                CleanUp(srtVars, srtResults);
                srtProgress = null;
                throw;
            }
            return srtResults;
        }


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

        {
            return SortFixedWidthByKeyCore<string>(sourcefilePath: sourcefilePath,
                                   getKey: getKey,
                                   dataFilter: dataFilter,
                                   destinationFolder: destinationFolder,
                                   hasHeader: hasHeader,
                                   isUniqueKey: isUniqueKey,
                                   returnDuplicates: returnDuplicates,
                                   sortDir: sortDir,
                                   progress: progress);


        }

        private static void CleanUp(SortVars srtVars, SortResults srtResults)
        {
            SortFileHelpers.ExceptionCleanUp(srtVars, srtResults);
            srtVars = null;
            srtResults = null;

        }


        private static void ReportReadProgress(Action<SortProgress> progress, SortProgress srtProgress, int linesRead)
        {
            if (progress != null)
            {
                srtProgress.Counter = linesRead;
                progress(srtProgress);
            }
        }

        private static string GetHeader(bool hasHeader, StreamReader reader)
        {
            string hdr = string.Empty;
            if (hasHeader)
            {
                hdr = reader.ReadLine();
            }
            return hdr;
        }

        private static void ArgumentValidation<T>(string sourcefilePath, Func<string[], string, T> getKey, string delimiter, string destinationFolder)
        {
            if (getKey == null)
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { getKey }), "A GetKey function must be defined.");
            }
            ArgumentValidation(delimiter);
            ArgumentValidation(sourcefilePath, destinationFolder);
        }

        private static void ArgumentValidation<T>(string sourcefilePath, Func<string, T> getKey, string destinationFolder)
        {
            if (getKey == null)
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { getKey }), "A GetKey function must be defined.");
            }
            ArgumentValidation(sourcefilePath, destinationFolder);
        }

        private static void ArgumentValidation(string sourcefilePath, string destinationFolder)
        {
            if (string.IsNullOrWhiteSpace(sourcefilePath))
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { sourcefilePath }), "The sourceFilePath cannot be null or empty.");
            }
            if (!File.Exists(sourcefilePath))
            {
                throw new FileNotFoundException("The sourceFilePath , " + sourcefilePath + " , does not exist.");
            }
            if (destinationFolder != null && !Directory.Exists(destinationFolder))
            {
                throw new DirectoryNotFoundException("The destination folder, " + destinationFolder + " , does not exist.");
            }

        }

        private static void ArgumentValidation(string delimiter)
        {
            if (string.IsNullOrEmpty(delimiter))
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { delimiter }), "The delimiter can not be null or empty.");
            }

        }

    }
}
