using System.IO;

namespace Sortiously
{
    public class MergePurgeResults
    {
        public int AddCount { get; set; }

        public int DeleteCount { get; set; }

        public int UpdateCount { get; set; }

        public int IgnoreCount { get; set; }

        public int MatchesCount { get; set; }

        public string AddsFilePath { get; set; }

        public string DeletesFilePath { get; set; }

        public string UpdatesFilePath { get; set; }

        public string IgnoredFilePath { get; set; }

        public string NewMasterFilePath { get; set; }

        internal void InitFilePaths(string masterSourceFilePath, string detailSourceFile, string destinationFolder)
        {
            AddsFilePath = Path.Combine(destinationFolder, GetSubFileName(detailSourceFile, "adds"));
            DeletesFilePath = Path.Combine(destinationFolder, GetSubFileName(detailSourceFile, "deletes"));
            UpdatesFilePath = Path.Combine(destinationFolder, GetSubFileName(detailSourceFile, "updates"));
            IgnoredFilePath = Path.Combine(destinationFolder, GetSubFileName(detailSourceFile, "ignored"));
            NewMasterFilePath = Path.Combine(destinationFolder, GetSubFileName(masterSourceFilePath, "master"));
            DeleteActionFiles();
        }

        internal void DeleteActionFiles()
        {
            SortFileHelpers.DeleteFileIfExists(AddsFilePath);
            SortFileHelpers.DeleteFileIfExists(DeletesFilePath);
            SortFileHelpers.DeleteFileIfExists(UpdatesFilePath);
            SortFileHelpers.DeleteFileIfExists(IgnoredFilePath);
        }

        internal void ClearSubFilePaths()
        {
            AddsFilePath = string.Empty;
            DeletesFilePath = string.Empty;
            UpdatesFilePath = string.Empty;
            IgnoredFilePath = string.Empty;
        }

        internal void ClearSubFilesIfNoCount()
        {
            if (AddCount == 0)
            {
                SortFileHelpers.DeleteFileIfExists(AddsFilePath);
                AddsFilePath = string.Empty;
            }
            if (DeleteCount == 0)
            {
                SortFileHelpers.DeleteFileIfExists(DeletesFilePath);
                DeletesFilePath = string.Empty;
            }
            if (UpdateCount == 0)
            {
                SortFileHelpers.DeleteFileIfExists(UpdatesFilePath);
                UpdatesFilePath = string.Empty;
            }
            if (IgnoreCount == 0)
            {
                SortFileHelpers.DeleteFileIfExists(IgnoredFilePath);
                IgnoredFilePath = string.Empty;
            }
        }

        internal void RemoveSubFilesAndFilePaths() {
            DeleteActionFiles();
            ClearSubFilePaths();
        }

        internal static string GetSubFileName(string filePath, string actionName)
        {
            return Path.GetFileNameWithoutExtension(filePath) + "_" + actionName + Path.GetExtension(filePath);
        }

        internal void IncAddCount()
        {
            AddCount++;
        }

        internal void IncDeleteCount()
        {
            DeleteCount++;
            IncMatchesCount();
        }

        internal void IncUpdateCount()
        {
            UpdateCount++;
            IncMatchesCount();
        }

        internal void IncIgnoreCount()
        {
            IgnoreCount++;
            IncMatchesCount();
        }

        private void IncMatchesCount()
        {
            MatchesCount++;
        }

        internal void IncrementAction(MergePurgeAction mpAction)
        {
            switch (mpAction)
            {
                case MergePurgeAction.Add:
                    IncAddCount();
                    break;
                case MergePurgeAction.Delete:
                    IncDeleteCount();
                    break;
                case MergePurgeAction.Update:
                    IncUpdateCount();
                    break;
                default:
                    IncIgnoreCount();
                    break;
            }
        }
    }
}