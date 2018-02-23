using System;
using System.IO;
using System.Data.SQLite;
using Sortiously.StringExtns;

namespace Sortiously
{
    public class SortResults
    {
        public int LinesRead { get; set; }
        public int LinesSorted { get; set; }
        public int Duplicates { get; set; }
        public int Filtered { get; set; }
        public string SortedFilePath { get; set; }
        public string DuplicatesFilePath { get; set; }
        internal string DbConnPath { get; set; }
        internal string Header { get; set; }

        public SortResults(string sourceFilePath, string destFolder, string dbConnPath)
        {
            string sortedFileName = SortFileHelpers.GetSortedFileName(sourceFilePath);
            DuplicatesFilePath = Path.Combine(destFolder, SortFileHelpers.GetDupesFileName(sourceFilePath));
            SortedFilePath = Path.Combine(destFolder, sortedFileName);
            DbConnPath = dbConnPath;
        }

        internal void IncrementLinesRead()
        {
            LinesRead++;
        }

        internal void IncrementLinesSorted()
        {
            LinesSorted++;
        }


        internal void IncrementDuplicates()
        {
            Duplicates++;
        }

        internal void IncrementFiltered()
        {
            Filtered++;
        }

        internal void DeleteDuplicatesFile()
        {
            SortFileHelpers.DeleteFileIfExists(DuplicatesFilePath);
        }

        internal void DeleteDuplicatesFileIfNoDuplicates()
        {
            if (Duplicates == 0)
            {
                DeleteDuplicatesFile();
                DuplicatesFilePath = null;
            }
        }

        internal void DeleteSortedFile()
        {
            SortFileHelpers.DeleteFileIfExists(SortedFilePath);
        }

        private void ReportProgress(Action<int> progress, int linesSorted)
        {
            if (progress != null)
            {
                progress(linesSorted);
            }

        }




        internal void WriteOutSorted(string dbConnPath, string header, SortDirection sortDir, string delimiter = Constants.Delimiters.Comma, bool hasUniqueIndex = false, bool returnDuplicates = false, string dupesFilePath = "", bool compressed = false, Action<int> progress = null, bool deleteDb = true)
        {
            DeleteSortedFile();
            this.Header = header;
            StreamWriter dupeWriter = !string.IsNullOrEmpty(dupesFilePath) ? new StreamWriter(dupesFilePath) : null;
            using (StreamWriter sw = new StreamWriter(SortedFilePath))
            using (dupeWriter)
            {
                if (!string.IsNullOrWhiteSpace(header))
                {
                    sw.WriteLine(header);
                    if (returnDuplicates)
                    {
                        WriteHeaderForDuplicatesFile(true, header, dupeWriter);
                    }

                }

                using (var cn = new SQLiteConnection(@"Data Source=" + dbConnPath))
                {
                    string selectCmd = "SELECT * FROM FileData ORDER BY SortKey";
                    if (sortDir == SortDirection.Descending)
                    {
                        selectCmd += " DESC";
                    }
                    cn.Open();
                    using (var cmd = new SQLiteCommand(selectCmd, cn))
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        dynamic lastReadKey = null;
                        while (rdr.Read())
                        {
                            string sqlLiteData = (string)rdr["LineData"];
                            string sqlLiteoutLine = SortFileHelpers.UnEscapeByDelimiter(compressed ? sqlLiteData.Decompress() : sqlLiteData, delimiter);
                            if (hasUniqueIndex)
                            {
                                dynamic sqlLiteKey = rdr["SortKey"];
                                if (sqlLiteKey.Equals(lastReadKey))
                                {
                                    if (returnDuplicates)
                                    {
                                        dupeWriter.WriteLine(sqlLiteoutLine);
                                        this.IncrementDuplicates();
                                    }
                                    continue;
                                }
                                lastReadKey = sqlLiteKey;
                            }
                            sw.WriteLine(sqlLiteoutLine);
                            IncrementLinesSorted();
                            ReportProgress(progress, LinesSorted);
                        }
                    }
                    cn.Close();
                }

            }
            if (deleteDb)
            {
                SortFileHelpers.DeleteFileIfExists(dbConnPath);
            }

        }

        private void WriteHeaderForDuplicatesFile(bool hasHeader, string hdr, StreamWriter dupeWriter)
        {
            if (hasHeader && !string.IsNullOrEmpty(hdr))
            {
                dupeWriter.WriteLine(hdr);
            }
        }


    }

}

