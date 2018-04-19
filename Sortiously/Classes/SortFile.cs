using System;
using System.IO;
using Sortiously.StringExtns;
using Sortiously.Framework;

namespace Sortiously
{
    public static class SortFile
    {
        /// <summary>
        /// Sorts a delimited file given a numeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="delimiter">Character delimiter</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="keyColumn">The zero based column number to be used as the key to sort</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
        public static SortResults SortDelimitedByNumericKey(string sourcefilePath,
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
        {

            SortDefinitions sortDefs = new SortDefinitions();
            sortDefs.Add(new SortDefinition { DataType = KeyType.Numberic, Direction = sortDir, IsUniqueKey = isUniqueKey });

            return SortDelimitedByKeyDefCore(sourcefilePath: sourcefilePath,
                                             sortDefinitions: sortDefs,
                                             setKeys: (fields, line, keyValues) => keyValues[0] = fields[keyColumn],
                                             dataFilter: dataFilter,
                                             destinationFolder: destinationFolder,
                                             delimiter: delimiter,
                                             hasHeader: hasHeader,
                                             returnDuplicates: returnDuplicates,
                                             dataTransportation: dataTransportation,
                                             progress: progress,
                                             maxBatchSize: maxBatchSize);
        }



        /// <summary>
        /// Sorts a delimited file given a alphanumeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="delimiter">Character delimiter</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="keyColumn">The zero based column number to be used as the key to sort</param>
        /// <param name="keyLength">The length of the key right justified with zeros if less than length specified</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
        public static SortResults SortDelimitedByAlphaNumKey(string sourcefilePath,
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

        {
            SortDefinitions sortDefs = new SortDefinitions();
            sortDefs.Add(new SortDefinition { DataType = KeyType.AlphaNumeric, Direction = sortDir, IsUniqueKey = isUniqueKey });

            return SortDelimitedByKeyDefCore(sourcefilePath: sourcefilePath,
                                            sortDefinitions: sortDefs,
                                            setKeys: (fields, line, keyValues) => keyValues[0] = fields[keyColumn].PadKeyWithZero(keyLength),
                                            dataFilter: dataFilter,
                                            destinationFolder: destinationFolder,
                                            delimiter: delimiter,
                                            hasHeader: hasHeader,
                                            returnDuplicates: returnDuplicates,
                                            dataTransportation: dataTransportation,
                                            progress: progress,
                                            maxBatchSize: maxBatchSize);
        }

        /// <summary>
        /// Sorts a delimited file given a numeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="getKey">Function to construct the key</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="delimiter">Character delimiter</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
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

        {


            SortDefinitions sortDefs = new SortDefinitions();
            sortDefs.Add(new SortDefinition { DataType = KeyType.Numberic, Direction = sortDir, IsUniqueKey = isUniqueKey });

            return SortDelimitedByKeyDefCore(sourcefilePath: sourcefilePath,
                                             sortDefinitions: sortDefs,
                                             setKeys: (fields, line, keyValues) => keyValues[0] = getKey(fields, line).ToString(),
                                             dataFilter: dataFilter,
                                             destinationFolder: destinationFolder,
                                             delimiter: delimiter,
                                             hasHeader: hasHeader,
                                             returnDuplicates: returnDuplicates,
                                             dataTransportation: dataTransportation,
                                             progress: progress,
                                             maxBatchSize: maxBatchSize);
        }


        /// <summary>
        /// Sorts a delimited file given a alphanumeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="getKey">Function to construct the key</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="delimiter">Character delimiter</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
        public static SortResults SortDelimitedByAlphaNumKey(string sourcefilePath,
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

        {

            SortDefinitions sortDefs = new SortDefinitions();
            sortDefs.Add(new SortDefinition { DataType = KeyType.AlphaNumeric, Direction = sortDir, IsUniqueKey = isUniqueKey });

            return SortDelimitedByKeyDefCore(sourcefilePath: sourcefilePath,
                                            sortDefinitions: sortDefs,
                                            setKeys: (fields, line, keyValues) => keyValues[0] = getKey(fields, line),
                                            dataFilter: dataFilter,
                                            destinationFolder: destinationFolder,
                                            delimiter: delimiter,
                                            hasHeader: hasHeader,
                                            returnDuplicates: returnDuplicates,
                                            dataTransportation: dataTransportation,
                                            progress: progress,
                                            maxBatchSize: maxBatchSize);

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
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
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
                                   DataTransportation dataTransportation = null,
                                   bool deleteDbConnPath = true,
                                   bool writeOutSortFile = true,
                                   int maxBatchSize = 250000)
        {
            ArgumentValidation.Validate<T>(sourcefilePath, getKey,  delimiter,  destinationFolder, maxBatchSize);
            SortVars srtVars = new SortVars(sourcefilePath, destinationFolder);
            SortResults srtResults = new SortResults(sourcefilePath, srtVars.DestFolder, srtVars.DbConnPath);
            SortProgress srtProgress = new SortProgress();
            try
            {
                srtResults.DeleteDuplicatesFile();
                int lineCount = 1;
                using (StreamReader reader = new StreamReader(sourcefilePath))
                using (SqliteSortKeyBulkInserter<T> sortBulkInserter = new SqliteSortKeyBulkInserter<T>(srtVars.DbConnPath, uniqueKey: isUniqueKey, maxBatchSize: maxBatchSize))
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
                    srtResults.WriteOutSorted(srtVars.DbConnPath, srtVars.Header, sortDir, delimiter, hasUniqueIndex: isUniqueKey, returnDuplicates: returnDuplicates, dupesFilePath: srtResults.DuplicatesFilePath, progress: (counter) => { srtProgress.Counter = counter; if (progress != null) { progress(srtProgress); } }, dataTransportation: dataTransportation, deleteDb: deleteDbConnPath);
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
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="keyDef">The zero based starting position of the key and length, will be trimmed.</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
        public static SortResults SortFixedWidthByNumericKey(string sourcefilePath,
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

        {
            SortDefinitions sortDefs = new SortDefinitions();
            sortDefs.Add(new SortDefinition { DataType = KeyType.Numberic, Direction = sortDir, IsUniqueKey = isUniqueKey });

            return SortFile.SortFixedWidthByKeyDefinitions(
                                   sourcefilePath: sourcefilePath,
                                   sortDefinitions: sortDefs,
                                   dataFilter: dataFilter,
                                   setKeys: (line, keyValues) => keyValues[0] = line.Substring(keyDef.StartPos, keyDef.KeyLength).Trim(),
                                   dataTransportation: dataTransportation,
                                   destinationFolder: destinationFolder,
                                   hasHeader: hasHeader,
                                   returnDuplicates: returnDuplicates,
                                   progress: progress,
                                   maxBatchSize: maxBatchSize);

        }

        /// <summary>
        /// Sorts a fixed width file given a alphanumeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="keyDef">The zero based starting position of the key and length, will be trimmed.</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
        public static SortResults SortFixedWidthByAlphaNumKey(string sourcefilePath,
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
        {
            SortDefinitions sortDefs = new SortDefinitions();
            sortDefs.Add(new SortDefinition { DataType = KeyType.AlphaNumeric, Direction = sortDir, IsUniqueKey = isUniqueKey });

            return SortFile.SortFixedWidthByKeyDefinitions(
                                   sourcefilePath: sourcefilePath,
                                   sortDefinitions: sortDefs,
                                   dataFilter: dataFilter,
                                   setKeys: (line, keyValues) => keyValues[0] = line.Substring(keyDef.StartPos, keyDef.KeyLength).PadKeyWithZero(keyDef.KeyLength),
                                   dataTransportation: dataTransportation,
                                   destinationFolder: destinationFolder,
                                   hasHeader: hasHeader,
                                   returnDuplicates: returnDuplicates,
                                   progress: progress,
                                   maxBatchSize: maxBatchSize);

        }


        /// <summary>
        /// Sorts a fixed width file given a numeric key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="getKey">Function to construct the key</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
        public static SortResults SortFixedWidthByNumericKey(string sourcefilePath,
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

        {
            SortDefinitions sortDefs = new SortDefinitions();
            sortDefs.Add(new SortDefinition { DataType = KeyType.Numberic, Direction = sortDir, IsUniqueKey = isUniqueKey });

            return SortFile.SortFixedWidthByKeyDefinitions(
                                   sourcefilePath: sourcefilePath,
                                   sortDefinitions: sortDefs,
                                   dataFilter: dataFilter,
                                   setKeys: (line, keyValues) => keyValues[0] = getKey(line).ToString(),
                                   dataTransportation: dataTransportation,
                                   destinationFolder: destinationFolder,
                                   hasHeader: hasHeader,
                                   returnDuplicates: returnDuplicates,
                                   progress: progress,
                                   maxBatchSize: maxBatchSize);
        }


        /// <summary>
        /// Sorts a fixed width file given a numeric or string key.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="getKey">Function to construct the key</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
        internal static SortResults SortFixedWidthByKeyCore<T>(string sourcefilePath,
                                   Func<string, T> getKey,
                                   Func<string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   bool hasHeader = true,
                                   bool isUniqueKey = false,
                                   bool returnDuplicates = false,
                                   SortDirection sortDir = SortDirection.Ascending,
                                   Action<SortProgress> progress = null,
                                   DataTransportation dataTransportation = null,
                                   bool deleteDbConnPath = true,
                                   bool writeOutSortFile = true,
                                   int maxBatchSize = 250000)

        {
            ArgumentValidation.Validate<T>(sourcefilePath, getKey, destinationFolder, maxBatchSize);
            SortVars srtVars = new SortVars(sourcefilePath, destinationFolder);
            SortResults srtResults = new SortResults(sourcefilePath, srtVars.DestFolder, srtVars.DbConnPath);
            SortProgress srtProgress = new SortProgress();
            try
            {
                srtResults.DeleteDuplicatesFile();
                int lineCount = 1;
                using (StreamReader reader = new StreamReader(sourcefilePath))
                using (SqliteSortKeyBulkInserter<T> sortBulkInserter = new SqliteSortKeyBulkInserter<T>(srtVars.DbConnPath, uniqueKey: isUniqueKey, maxBatchSize: maxBatchSize))

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
                    srtResults.WriteOutSorted(dbConnPath: srtVars.DbConnPath, header: srtVars.Header, sortDir: sortDir, delimiter: Constants.Delimiters.Tab, hasUniqueIndex: isUniqueKey, returnDuplicates: returnDuplicates, dupesFilePath: srtResults.DuplicatesFilePath, compressed: true, progress: (counter) => { srtProgress.Counter = counter; if (progress != null) { progress(srtProgress); } }, dataTransportation: dataTransportation, deleteDb: deleteDbConnPath);
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
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="isUniqueKey">If true duplicates will not be included in the sorted file.</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true.</param>
        /// <param name="sortDir">The sort direction of the key.</param>
        /// <param name="progress">A method to report progress</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
        public static SortResults SortFixedWidthByAlphaNumKey(string sourcefilePath,
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

        {
            SortDefinitions sortDefs = new SortDefinitions();
            sortDefs.Add(new SortDefinition { DataType = KeyType.AlphaNumeric, Direction = sortDir, IsUniqueKey = isUniqueKey });

            return SortFile.SortFixedWidthByKeyDefinitions(
                                   sourcefilePath: sourcefilePath,
                                   sortDefinitions: sortDefs,
                                   dataFilter: dataFilter,
                                   setKeys: (line, keyValues) => keyValues[0] = getKey(line),
                                   dataTransportation: dataTransportation,
                                   destinationFolder: destinationFolder,
                                   hasHeader: hasHeader,
                                   returnDuplicates: returnDuplicates,
                                   progress: progress,
                                   maxBatchSize: maxBatchSize);


        }

        /// <summary>
        /// Sorts a delimited file by defined set of key definitions.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="sortDefinitions">Define the keys values and sort directions</param>
        /// <param name="setKeys">Action method to set the key values</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="delimiter">Character delimiter</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true in any of the key definitions.</param>
        /// <param name="progress">A method to report progress</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
        /// <returns></returns>
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

        {
            return SortDelimitedByKeyDefCore(
                                   sourcefilePath: sourcefilePath,
                                   sortDefinitions: sortDefinitions,
                                   setKeys: setKeys,
                                   dataFilter: dataFilter,
                                   destinationFolder: destinationFolder,
                                   delimiter: delimiter,
                                   hasHeader: hasHeader,
                                   returnDuplicates: returnDuplicates,
                                   progress: progress,
                                   maxBatchSize: maxBatchSize,
                                   dataTransportation: dataTransportation);
        }

        internal static SortResults SortDelimitedByKeyDefCore(
                                   string sourcefilePath,
                                   SortDefinitions sortDefinitions,
                                   Action<string[], string, string[]> setKeys,
                                   Func<string[], string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   string delimiter = Constants.Delimiters.Comma,
                                   bool hasHeader = true,
                                   bool returnDuplicates = false,
                                   Action<SortProgress> progress = null,
                                   DataTransportation dataTransportation = null,
                                   bool deleteDbConnPath = true,
                                   bool writeOutSortFile = true,
                                   int maxBatchSize = 250000)

        {
            ArgumentValidation.Validate(sourcefilePath, setKeys, delimiter, destinationFolder);
            SortVars srtVars = new SortVars(sourcefilePath, destinationFolder);
            SortResults srtResults = new SortResults(sourcefilePath, srtVars.DestFolder, srtVars.DbConnPath);
            SortProgress srtProgress = new SortProgress();
            try
            {
                srtResults.DeleteDuplicatesFile();
                int lineCount = 1;
                using (StreamReader reader = new StreamReader(sourcefilePath))
                using (SqliteSortDefBulkInserter sortBulkInserter = new SqliteSortDefBulkInserter(srtVars.DbConnPath, sortDefinitions, maxBatchSize))
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

                                string[] keyValues = new string[sortDefinitions.GetKeys().Count];
                                setKeys(fields, line, keyValues);
                                sortBulkInserter.Add(new SortKeyData { KeyValues = keyValues, Data = line + SortFileHelpers.EscapeByDelimiter(delimiter) });
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
                    srtResults.WriteOutSorted(srtVars.DbConnPath,
                                              srtVars.Header,
                                              sortDefinitions,
                                              delimiter,
                                              returnDuplicates: returnDuplicates,
                                              dupesFilePath: srtResults.DuplicatesFilePath,
                                              progress: (counter) => { srtProgress.Counter = counter; if (progress != null) { progress(srtProgress); } },
                                              dataTransportation: dataTransportation,
                                              deleteDb: deleteDbConnPath);
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
        /// Sorts a fixed width file by defined set of key definitions.
        /// </summary>
        /// <param name="sourcefilePath">Full path and file name of file to be sorted</param>
        /// <param name="sortDefinitions">Define the keys values and sort directions</param>
        /// <param name="setKeys">Action method to set the key values</param>
        /// <param name="dataTransportation">Define the data transportation method.</param>
        /// <param name="dataFilter">Function to filter out a data line (true to include data or false to exclude data)</param>
        /// <param name="destinationFolder">Folder path where sorted and/or duplicate files will be place. (Uses folder of sourcefilePath when null)</param>
        /// <param name="hasHeader">Does the file have a header row</param>
        /// <param name="returnDuplicates">If true duplicates will be written out to file only if isUniqueKey is true in any of the key definitions.</param>
        /// <param name="progress">A method to report progress</param>
        /// <param name="maxBatchSize">Control the max insert batch size</param>
        /// <returns></returns>
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
        {

            return SortFixedWidthByKeyDefCore(sourcefilePath: sourcefilePath,
                                   sortDefinitions: sortDefinitions,
                                   setKeys: setKeys,
                                   dataFilter: dataFilter,
                                   destinationFolder: destinationFolder,
                                   hasHeader: hasHeader,
                                   returnDuplicates: returnDuplicates,
                                   progress: progress,
                                   maxBatchSize: maxBatchSize,
                                   dataTransportation: dataTransportation);
        }

        internal static SortResults SortFixedWidthByKeyDefCore(string sourcefilePath,
                                   SortDefinitions sortDefinitions,
                                   Action<string, string[]> setKeys,
                                   Func<string, bool> dataFilter = null,
                                   string destinationFolder = null,
                                   bool hasHeader = true,
                                   bool returnDuplicates = false,
                                   Action<SortProgress> progress = null,
                                   DataTransportation dataTransportation = null,
                                   bool deleteDbConnPath = true,
                                   bool writeOutSortFile = true,
                                   int maxBatchSize = 250000)
        {
            ArgumentValidation.Validate(sourcefilePath, setKeys, destinationFolder);
            SortVars srtVars = new SortVars(sourcefilePath, destinationFolder);
            SortResults srtResults = new SortResults(sourcefilePath, srtVars.DestFolder, srtVars.DbConnPath);
            SortProgress srtProgress = new SortProgress();
            try
            {
                srtResults.DeleteDuplicatesFile();
                int lineCount = 1;
                using (StreamReader reader = new StreamReader(sourcefilePath))
                using (SqliteSortDefBulkInserter sortBulkInserter = new SqliteSortDefBulkInserter(srtVars.DbConnPath, sortDefinitions, maxBatchSize))
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

                            string[] keyValues = new string[sortDefinitions.GetKeys().Count];
                            setKeys(line, keyValues);
                            sortBulkInserter.Add(new SortKeyData { KeyValues = keyValues, Data = (line + Constants.Common.PreserveCharacter).Compress() });
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

                    srtResults.WriteOutSorted(dbConnPath: srtVars.DbConnPath,
                                              header: srtVars.Header,
                                              sortDefinitions: sortDefinitions,
                                              delimiter: Constants.Delimiters.Tab,
                                              returnDuplicates: returnDuplicates,
                                              dupesFilePath: srtResults.DuplicatesFilePath,
                                              compressed: true,
                                              progress: (counter) => { srtProgress.Counter = counter; if (progress != null) { progress(srtProgress); } },
                                              dataTransportation: dataTransportation,
                                              deleteDb: deleteDbConnPath);
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


    }
}
