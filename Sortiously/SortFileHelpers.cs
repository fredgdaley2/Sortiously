using System.IO;

namespace Sortiously
{
    internal static class SortFileHelpers
    {
        internal static void DeleteFileIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        internal static string EscapeByDelimiter(string delimiter)
        {
            return delimiter == Constants.Delimiters.Tab ? Constants.Common.PreserveCharacter.ToString() : "";
        }

        internal static string UnEscapeByDelimiter(string data, string delimiter)
        {
            return delimiter == Constants.Delimiters.Tab ? data.Substring(0, data.Length - 1) : data;
        }

        internal static void ExceptionCleanUp(SortVars srtVars, SortResults srtResults)
        {
            DeleteFileIfExists(srtVars.DbConnPath);
            DeleteFileIfExists(Path.Combine(srtVars.DestFolder, srtVars.DbJrnFileName));
            srtResults.DeleteSortedFile();
            srtResults.DeleteDuplicatesFile();

        }

        internal static string GetRandomDbConnStr()
        {
            return Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".db";
        }

        internal static string GetDbJournalName(string dbConnStr)
        {
            return Path.GetFileNameWithoutExtension(dbConnStr) + "-journal.db";
        }

        internal static string GetSortedFileName(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath) + "_sorted" + Path.GetExtension(filePath);
        }

        internal static string GetDupesFileName(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath) + "_dupes" + Path.GetExtension(filePath);
        }


        internal static string GetSourceDirName(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }

        internal static string GetDestinationFolder(string sourceFolder, string destinationFolder)
        {
            return !string.IsNullOrEmpty(destinationFolder) ? destinationFolder : sourceFolder;
        }
    }
}
