using System.IO;


namespace Sortiously
{
    internal class SortVars
    {
        public string DbConnStr { get; set; }
        public string DbJrnFileName { get; set; }
        public string SortedFileName { get; set; }
        public string SourceDirName { get; set; }
        public string DestFolder { get; set; }
        public string DbConnPath { get; set; }
        public string Header { get; set; }


        public SortVars(string sourceFilePath, string destFolder )
        {
            DbConnStr = SortFileHelpers.GetRandomDbConnStr();
            DbJrnFileName = SortFileHelpers.GetDbJournalName(DbConnStr);
            SortedFileName = SortFileHelpers.GetSortedFileName(sourceFilePath);
            SourceDirName = SortFileHelpers.GetSourceDirName(sourceFilePath);
            DestFolder = SortFileHelpers.GetDestinationFolder(SourceDirName, destFolder);
            DbConnPath = Path.Combine(DestFolder, DbConnStr);
        }
    }
}
