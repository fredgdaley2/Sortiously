using System;
using System.IO;
using Sortiously.StringExtns;
using Sortiously.Interfaces;
using Sortiously.Framework;

namespace Sortiously
{
    public static class MasterDetail
    {
        public static MergePurgeResults MergePurge<T>(MasterDelimitedFileSource<T> master,
                                                      DelimitedFileSource<T> detail,
                                                      Action<MergePurgeParam> processData,
                                                      string destinationFolder = null,
                                                      DataMode processMode = DataMode.Passive)
        {
            ArgumentValidation<T>(master, detail, processData, destinationFolder);
            MergePurgeResults mgPurgeResults = new MergePurgeResults();
            SortVars mstSortVars = new SortVars(master.SourceFilePath, destinationFolder);
            SortResults srtResults = SortDelimited<T>(master, mstSortVars.DestFolder);
            mgPurgeResults.InitFilePaths(master.SourceFilePath, detail.SourceFilePath, mstSortVars.DestFolder);
            try
            {
                string hdr = string.Empty;
                using (StreamReader reader = new StreamReader(detail.SourceFilePath))
                using (StreamWriter addSw = new StreamWriter(mgPurgeResults.AddsFilePath))
                using (StreamWriter delSw = new StreamWriter(mgPurgeResults.DeletesFilePath))
                using (StreamWriter updSw = new StreamWriter(mgPurgeResults.UpdatesFilePath))
                using (StreamWriter ignSw = new StreamWriter(mgPurgeResults.IgnoredFilePath))
                {
                    StreamWriter[] actionWriters = { addSw, delSw, updSw, ignSw };
                    string line;
                    hdr = GetHeader(detail.HasHeader, reader);
                    WriteHeaderToActionWriters(processMode, detail.HasHeader, hdr, actionWriters);
                    while ((line = reader.ReadLine()) != null)
                    {
                        MergePurgeParam mgPurgeParam = new MergePurgeParam();
                        FileParser.ParseDelimitedString(new StringReader(line), (fields, lNum) =>
                        {
                            mgPurgeParam.DetailFields = fields;
                            mgPurgeParam.DataAction = MergePurgeAction.Ignore;
                        }, detail.Delimiter);
                        DelimitedSortMurgePurge<T>(srtResults.DbConnPath, line, master, detail, mgPurgeParam, processData, mgPurgeResults, processMode, actionWriters);
                    }
                }
                mgPurgeResults.ClearSubFilesIfNoCount();
                if (processMode == DataMode.Active)
                {
                    mgPurgeResults.RemoveSubFilesAndFilePaths();
                }
                srtResults.SortedFilePath = mgPurgeResults.NewMasterFilePath;
                srtResults.WriteOutSorted(dbConnPath: srtResults.DbConnPath, header: srtResults.Header, sortDir: master.SortDirection, delimiter: master.Delimiter, deleteDb: true);
            }
            catch (Exception)
            {
                ExceptionCleanUp(srtResults.DbConnPath, mgPurgeResults);
                throw;
            }

            return mgPurgeResults;
        }



        public static MergePurgeResults MergePurge<T>(MasterDelimitedFileSource<T> master,
                                                      FixedWidthFileSource<T> detail,
                                                      Action<MergePurgeParam> processData,
                                                      string destinationFolder = null,
                                                      DataMode processMode = DataMode.Passive)
        {
            ArgumentValidation<T>(master, detail, processData, destinationFolder);
            MergePurgeResults mgPurgeResults = new MergePurgeResults();
            SortVars mstSortVars = new SortVars(master.SourceFilePath, destinationFolder);
            SortResults srtResults = SortDelimited<T>(master, mstSortVars.DestFolder);
            mgPurgeResults.InitFilePaths(master.SourceFilePath, detail.SourceFilePath, mstSortVars.DestFolder);
            try
            {
                string hdr = string.Empty;
                using (StreamReader reader = new StreamReader(detail.SourceFilePath))
                using (StreamWriter addSw = new StreamWriter(mgPurgeResults.AddsFilePath))
                using (StreamWriter delSw = new StreamWriter(mgPurgeResults.DeletesFilePath))
                using (StreamWriter updSw = new StreamWriter(mgPurgeResults.UpdatesFilePath))
                using (StreamWriter ignSw = new StreamWriter(mgPurgeResults.IgnoredFilePath))
                {
                    StreamWriter[] actionWriters = { addSw, delSw, updSw, ignSw };
                    string line;
                    hdr = GetHeader(detail.HasHeader, reader);
                    WriteHeaderToActionWriters(processMode, detail.HasHeader, hdr, actionWriters);
                    while ((line = reader.ReadLine()) != null)
                    {
                        MergePurgeParam mgPurgeParam = new MergePurgeParam();
                        FileParser.ParseFixedWidthString(new StringReader(line), (fields, lNum) =>
                        {
                            mgPurgeParam.DetailFields = fields;
                            mgPurgeParam.DataAction = MergePurgeAction.Ignore;
                        }, detail.FixedWidths);
                        FixedWidthSortMurgePurge<T>(srtResults.DbConnPath, line, master, detail, mgPurgeParam, processData, mgPurgeResults, processMode, actionWriters);
                    }
                }
                mgPurgeResults.ClearSubFilesIfNoCount();
                if (processMode == DataMode.Active)
                {
                    mgPurgeResults.RemoveSubFilesAndFilePaths();
                }
                srtResults.SortedFilePath = mgPurgeResults.NewMasterFilePath;
                srtResults.WriteOutSorted(dbConnPath: srtResults.DbConnPath, header: srtResults.Header, sortDir: master.SortDirection, delimiter: master.Delimiter, deleteDb: true);
            }
            catch (Exception)
            {
                ExceptionCleanUp(srtResults.DbConnPath, mgPurgeResults);
                throw;
            }

            return mgPurgeResults;
        }

        public static MergePurgeResults MergePurge<T>(MasterFixedWidthFileSource<T> master,
                                                      FixedWidthFileSource<T> detail,
                                                      Action<MergePurgeParam> processData,
                                                      string destinationFolder = null,
                                                      DataMode processMode = DataMode.Passive)
        {
            ArgumentValidation<T>(master, detail, processData, destinationFolder);
            MergePurgeResults mgPurgeResults = new MergePurgeResults();
            SortVars mstSortVars = new SortVars(master.SourceFilePath, destinationFolder);
            SortResults srtResults = SortFixedWidth<T>(master, mstSortVars.DestFolder);
            mgPurgeResults.InitFilePaths(master.SourceFilePath, detail.SourceFilePath, mstSortVars.DestFolder);
            try
            {
                string hdr = string.Empty;
                using (StreamReader reader = new StreamReader(detail.SourceFilePath))
                using (StreamWriter addSw = new StreamWriter(mgPurgeResults.AddsFilePath))
                using (StreamWriter delSw = new StreamWriter(mgPurgeResults.DeletesFilePath))
                using (StreamWriter updSw = new StreamWriter(mgPurgeResults.UpdatesFilePath))
                using (StreamWriter ignSw = new StreamWriter(mgPurgeResults.IgnoredFilePath))
                {
                    StreamWriter[] actionWriters = { addSw, delSw, updSw, ignSw };
                    string line;
                    hdr = GetHeader(detail.HasHeader, reader);
                    WriteHeaderToActionWriters(processMode, detail.HasHeader, hdr, actionWriters);
                    while ((line = reader.ReadLine()) != null)
                    {
                        MergePurgeParam mgPurgeParam = new MergePurgeParam();
                        FileParser.ParseFixedWidthString(new StringReader(line), (fields, lNum) =>
                        {
                            mgPurgeParam.DetailFields = fields;
                            mgPurgeParam.DataAction = MergePurgeAction.Ignore;
                        }, detail.FixedWidths);
                        FixedWidthSortMurgePurge<T>(srtResults.DbConnPath, line, master, detail, mgPurgeParam, processData, mgPurgeResults, processMode, actionWriters);
                    }
                }
                mgPurgeResults.ClearSubFilesIfNoCount();
                if (processMode == DataMode.Active)
                {
                    mgPurgeResults.RemoveSubFilesAndFilePaths();
                }
                srtResults.SortedFilePath = mgPurgeResults.NewMasterFilePath;
                srtResults.WriteOutSorted(dbConnPath: srtResults.DbConnPath, header: srtResults.Header, sortDir: master.SortDirection, delimiter: Constants.Delimiters.Tab,compressed: true, deleteDb: true);

            }
            catch (Exception)
            {
                ExceptionCleanUp(srtResults.DbConnPath, mgPurgeResults);
                throw;
            }

            return mgPurgeResults;
        }

        public static MergePurgeResults MergePurge<T>(MasterFixedWidthFileSource<T> master,
                                                      DelimitedFileSource<T> detail,
                                                      Action<MergePurgeParam> processData,
                                                      string destinationFolder = null,
                                                      DataMode processMode = DataMode.Passive)
        {
            ArgumentValidation<T>(master, detail, processData, destinationFolder);
            MergePurgeResults mgPurgeResults = new MergePurgeResults();
            SortVars mstSortVars = new SortVars(master.SourceFilePath, destinationFolder);
            SortResults srtResults = SortFixedWidth<T>(master, mstSortVars.DestFolder);
            mgPurgeResults.InitFilePaths(master.SourceFilePath, detail.SourceFilePath, mstSortVars.DestFolder);
            try
            {
                string hdr = string.Empty;
                using (StreamReader reader = new StreamReader(detail.SourceFilePath))
                using (StreamWriter addSw = new StreamWriter(mgPurgeResults.AddsFilePath))
                using (StreamWriter delSw = new StreamWriter(mgPurgeResults.DeletesFilePath))
                using (StreamWriter updSw = new StreamWriter(mgPurgeResults.UpdatesFilePath))
                using (StreamWriter ignSw = new StreamWriter(mgPurgeResults.IgnoredFilePath))
                {
                    StreamWriter[] actionWriters = { addSw, delSw, updSw, ignSw };
                    string line;
                    hdr = GetHeader(detail.HasHeader, reader);
                    WriteHeaderToActionWriters(processMode, detail.HasHeader, hdr, actionWriters);
                    while ((line = reader.ReadLine()) != null)
                    {
                        MergePurgeParam mgPurgeParam = new MergePurgeParam();
                        FileParser.ParseDelimitedString(new StringReader(line), (fields, lNum) =>
                        {
                            mgPurgeParam.DetailFields = fields;
                            mgPurgeParam.DataAction = MergePurgeAction.Ignore;
                        }, detail.Delimiter);
                        FixedWidthSortMurgePurge<T>(srtResults.DbConnPath, line, master, detail, mgPurgeParam, processData, mgPurgeResults, processMode, actionWriters);
                    }
                }
                mgPurgeResults.ClearSubFilesIfNoCount();
                if (processMode == DataMode.Active)
                {
                    mgPurgeResults.RemoveSubFilesAndFilePaths();
                }
                srtResults.SortedFilePath = mgPurgeResults.NewMasterFilePath;
                srtResults.WriteOutSorted(dbConnPath: srtResults.DbConnPath, header: srtResults.Header, sortDir: master.SortDirection, delimiter: Constants.Delimiters.Tab, compressed: true, deleteDb: true);

            }
            catch (Exception)
            {
                ExceptionCleanUp(srtResults.DbConnPath, mgPurgeResults);
                throw;
            }

            return mgPurgeResults;
        }



        internal static SortResults SortFixedWidth<T>(MasterFixedWidthFileSource<T> master, string destinationFolder)
        {
            return SortFile.SortFixedWidthByKeyCore<T>(sourcefilePath: master.SourceFilePath,
                getKey: master.GetKey,
                destinationFolder: destinationFolder,
                hasHeader: master.HasHeader,
                isUniqueKey: false,
                sortDir: master.SortDirection,
                deleteDbConnPath: false,
                writeOutSortFile: false);
        }

        internal static SortResults SortDelimited<T>(MasterDelimitedFileSource<T> master, string destinationFolder)
        {
            return SortFile.SortDelimitedByKeyCore<T>(sourcefilePath: master.SourceFilePath,
                    getKey: master.GetKey,
                    destinationFolder: destinationFolder,
                    delimiter: master.Delimiter,
                    hasHeader: master.HasHeader,
                    isUniqueKey: false,
                    sortDir: master.SortDirection,
                    deleteDbConnPath: false,
                    writeOutSortFile: false);
        }

        private static void DelimitedSortMurgePurge<T>(string dbConnPath, string line,
            MasterDelimitedFileSource<T> master,
            DelimitedFileSource<T> detail,
            MergePurgeParam mgPurgeParam,
            Action<MergePurgeParam> processData,
            MergePurgeResults mgPurgeResults,
            DataMode processMode,
            StreamWriter[] actionWriters)
        {
            SortKey<T> srtKey = null;
            using (SqliteRepository<T> sqlRepo = new SqliteRepository<T>(dbConnPath))
            {
                srtKey = sqlRepo.KeyInDb(detail.GetKey(mgPurgeParam.DetailFields, line));
            }

            mgPurgeParam.KeyFound = srtKey.Found;
            string masterData = !srtKey.Found ? string.Empty : srtKey.Data;

            if (mgPurgeParam.KeyFound)
            {
                mgPurgeParam.MasterFields = GetMasterFields<T>(master, masterData);
            }

            if (processData != null)
            {
                processData(mgPurgeParam);
                mgPurgeResults.IncrementAction(mpAction: mgPurgeParam.DataAction);
                string detailLine = GetDetailLine<T>(detail, mgPurgeParam);
                string newMasterData = GetNewMasterDelimitedData<T>(master, GetMasterLine<T>(master, mgPurgeParam));
                PerformDataActionForPassiveModeWriter(mgPurgeParam.DataAction, mgPurgeParam.KeyFound, processMode, detailLine, actionWriters);
                PerformDataAction<T>(mgPurgeParam, dbConnPath, processMode, newMasterData, srtKey);

            }
        }

        private static void FixedWidthSortMurgePurge<T>(string dbConnPath, string line,
            MasterDelimitedFileSource<T> master,
            FixedWidthFileSource<T> detail,
            MergePurgeParam mgPurgeParam,
            Action<MergePurgeParam> processData,
            MergePurgeResults mgPurgeResults,
            DataMode processMode,
            StreamWriter[] actionWriters)
        {
            SortKey<T> srtKey = null;
            using (SqliteRepository<T> sqlRepo = new SqliteRepository<T>(dbConnPath))
            {
                srtKey = sqlRepo.KeyInDb(detail.GetKey(line));
            }

            mgPurgeParam.KeyFound = srtKey.Found;
            string masterData = !srtKey.Found ? string.Empty : srtKey.Data;

            if (mgPurgeParam.KeyFound)
            {
                mgPurgeParam.MasterFields = GetMasterFields<T>(master, masterData);
            }
            if (processData != null)
            {
                processData(mgPurgeParam);
                mgPurgeResults.IncrementAction(mpAction: mgPurgeParam.DataAction);
                string detailLine = GetDetailLine<T>(detail, mgPurgeParam);
                string newMasterData = GetNewMasterDelimitedData<T>(master, GetMasterLine<T>(master, mgPurgeParam));
                PerformDataActionForPassiveModeWriter(mgPurgeParam.DataAction, mgPurgeParam.KeyFound, processMode, detailLine, actionWriters);
                PerformDataAction(mgPurgeParam, dbConnPath, processMode, newMasterData, srtKey);
            }
        }


        private static void FixedWidthSortMurgePurge<T>(string dbConnPath, string line,
            MasterFixedWidthFileSource<T> master,
            FixedWidthFileSource<T> detail,
            MergePurgeParam mgPurgeParam,
            Action<MergePurgeParam> processData,
            MergePurgeResults mgPurgeResults,
            DataMode processMode,
            StreamWriter[] actionWriters)
        {

            SortKey<T> srtKey = null;
            using (SqliteRepository<T> sqlRepo = new SqliteRepository<T>(dbConnPath))
            {
                srtKey = sqlRepo.KeyInDb(detail.GetKey(line));
            }

            mgPurgeParam.KeyFound = srtKey.Found;
            string masterData = !srtKey.Found ? string.Empty : srtKey.Data;


            if (mgPurgeParam.KeyFound)
            {
                mgPurgeParam.MasterFields = GetMasterFields<T>(master, masterData);
            }

            if (processData != null)
            {
                processData(mgPurgeParam);
                mgPurgeResults.IncrementAction(mpAction: mgPurgeParam.DataAction);
                string detailLine = GetDetailLine<T>(detail, mgPurgeParam);
                string newMasterData = GetNewMasterDataFixedWidth(GetMasterLine<T>(master, mgPurgeParam));
                PerformDataActionForPassiveModeWriter(mgPurgeParam.DataAction, mgPurgeParam.KeyFound, processMode, detailLine, actionWriters);
                PerformDataAction(mgPurgeParam, dbConnPath, processMode, newMasterData, srtKey);
            }
        }

        private static void FixedWidthSortMurgePurge<T>(string dbConnPath, string line,
            MasterFixedWidthFileSource<T> master,
            DelimitedFileSource<T> detail,
            MergePurgeParam mgPurgeParam,
            Action<MergePurgeParam> processData,
            MergePurgeResults mgPurgeResults,
            DataMode processMode,
            StreamWriter[] actionWriters)
        {
            SortKey<T> srtKey = null;

            using (SqliteRepository<T> sqlRepo = new SqliteRepository<T>(dbConnPath))
            {
                srtKey = sqlRepo.KeyInDb(detail.GetKey(mgPurgeParam.DetailFields, line));
            }
            mgPurgeParam.KeyFound = srtKey.Found;
            string masterData = !srtKey.Found ? string.Empty : srtKey.Data;

            if (mgPurgeParam.KeyFound)
            {
                mgPurgeParam.MasterFields = GetMasterFields<T>(master, masterData);
            }
            if (processData != null)
            {
                processData(mgPurgeParam);
                mgPurgeResults.IncrementAction(mpAction: mgPurgeParam.DataAction);
                string detailLine = GetDetailLine<T>(detail, mgPurgeParam);
                string newMasterData = GetNewMasterDataFixedWidth(GetMasterLine<T>(master, mgPurgeParam));
                PerformDataActionForPassiveModeWriter(mgPurgeParam.DataAction, mgPurgeParam.KeyFound, processMode, detailLine, actionWriters);
                PerformDataAction(mgPurgeParam, dbConnPath, processMode, newMasterData, srtKey);
            }
        }


        private static void PerformDataActionForPassiveModeWriter(MergePurgeAction dataAction, bool keyFound, DataMode processMode, string detailLine, StreamWriter[] actionWriters)
        {
            StreamWriter actionWriter = null;
            switch (dataAction)
            {
                case MergePurgeAction.Add:
                    if (!keyFound)
                    {
                        actionWriter = actionWriters[(int)ActionWriter.AddsWriter];
                    }
                    break;
                case MergePurgeAction.Delete:
                    if (keyFound)
                    {
                        actionWriter = actionWriters[(int)ActionWriter.DeletesWriter];
                    }
                    break;
                case MergePurgeAction.Update:
                    if (keyFound)
                    {
                        actionWriter = actionWriters[(int)ActionWriter.UpdatesWriter];
                    }

                    break;
                default:
                    actionWriter = actionWriters[(int)ActionWriter.IgnoredWriter];
                    break;
            }

            if (actionWriter != null)
            {
                PassiveModeWriter(processMode, actionWriter, detailLine);
            }

        }

        private static void PerformDataAction<T>(MergePurgeParam mgPurgeParam,
            string dbConnPath,
            DataMode processMode,
            string newMasterData,
            SortKey<T> sortKey
)
        {
            using (SqliteRepository<T> sqlRepo = new SqliteRepository<T>(dbConnPath))
            {
                switch (mgPurgeParam.DataAction)
                {
                    case MergePurgeAction.Add:
                        if (!mgPurgeParam.KeyFound)
                        {
                            if (processMode == DataMode.Active)
                            {
                                sqlRepo.Add(sortKey.Key, newMasterData);
                            }
                        }
                        break;
                    case MergePurgeAction.Delete:
                        if (mgPurgeParam.KeyFound)
                        {
                            if (processMode == DataMode.Active)
                            {
                                sqlRepo.Delete(sortKey.Id);
                            }
                        }
                        break;
                    case MergePurgeAction.Update:
                        if (mgPurgeParam.KeyFound)
                        {
                            if (processMode == DataMode.Active)
                            {
                                sqlRepo.Update(sortKey.Id, newMasterData);
                            }
                        }

                        break;
                    default:
                        break;
                }
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

        private static void WriteHeaderToActionWriters(DataMode processMode, bool hasHeader, string hdr, StreamWriter[] actionWriters)
        {
            if (processMode == DataMode.Passive && hasHeader && !string.IsNullOrEmpty(hdr))
            {
                foreach (StreamWriter actionWriter in actionWriters)
                {
                    actionWriter.WriteLine(hdr);
                }
            }
        }


        private static void PassiveModeWriter(DataMode processMode, StreamWriter sw, string output)
        {
            if (processMode == DataMode.Passive)
            {
                sw.WriteLine(output);
            }

        }

        private static string[] GetMasterFields<T>(MasterDelimitedFileSource<T> master, string data)
        {
            string outLine = SortFileHelpers.UnEscapeByDelimiter(data, master.Delimiter);
            string[] results = null;
            FileParser.ParseDelimitedString(new StringReader(outLine), (fields, lNum) =>
            {
                results = fields;
            }, master.Delimiter);
            return results;
        }

        private static string[] GetMasterFields<T>(MasterFixedWidthFileSource<T> master, string data)
        {
            string outLine = SortFileHelpers.UnEscapeByDelimiter(data.Decompress(), Constants.Delimiters.Tab);
            string[] results = null;
            FileParser.ParseFixedWidthString(new StringReader(outLine), (fields, lNum) =>
            {
                results = fields;
            }, master.FixedWidths);
            return results;
        }

        private static string GetMasterLine<T>(MasterFixedWidthFileSource<T> master, MergePurgeParam mgPurgeParam)
        {
            return mgPurgeParam.MasterFields != null ? mgPurgeParam.MasterFields.SerializeToFixedWidth(master.FixedWidths) : string.Empty;
        }

        private static string GetMasterLine<T>(MasterDelimitedFileSource<T> master, MergePurgeParam mgPurgeParam)
        {
            return mgPurgeParam.MasterFields != null ? mgPurgeParam.MasterFields.SerializeToDelimited(master.Delimiter) : string.Empty;
        }

        private static string GetDetailLine<T>(DelimitedFileSource<T> detail, MergePurgeParam mgPurgeParam)
        {
            return mgPurgeParam.DetailFields.SerializeToDelimited(detail.Delimiter);
        }

        private static string GetDetailLine<T>(FixedWidthFileSource<T> detail, MergePurgeParam mgPurgeParam)
        {
            return mgPurgeParam.DetailFields.SerializeToFixedWidth(detail.FixedWidths);
        }

        private static string GetNewMasterDelimitedData<T>(MasterDelimitedFileSource<T> master, string masterLine)
        {
            return masterLine + SortFileHelpers.EscapeByDelimiter(master.Delimiter);
        }

        private static string GetNewMasterDataFixedWidth(string masterLine)
        {
            return (masterLine + Constants.Common.PreserveCharacter).Compress();
        }
        private static void ExceptionCleanUp(string dbConnPath, MergePurgeResults mgPurgeResults)
        {
            SortFileHelpers.DeleteFileIfExists(dbConnPath);
            SortFileHelpers.DeleteFileIfExists(Path.Combine(Path.GetDirectoryName(dbConnPath), SortFileHelpers.GetDbJournalName(dbConnPath)));
            mgPurgeResults.DeleteActionFiles();
            SortFileHelpers.DeleteFileIfExists(mgPurgeResults.NewMasterFilePath);
        }

        private static void ArgumentValidation<T>(MasterDelimitedFileSource<T> master, DelimitedFileSource<T> detail, Action<MergePurgeParam> processData, string destinationFolder = null)
        {
            if ((master.GetKey != null && detail.GetKey == null) || (master.GetKey == null && detail.GetKey != null))
            {
                throw new ArgumentException("Master and Detail must have the same GetKey function type defined.");
            }

            if (master.GetKey == null)
            {
                throw new ArgumentException("The Master File Source must have a GetKey function defined.");
            }
            if (detail.GetKey == null)
            {
                throw new ArgumentException("The Detail File Source must have a GetKey function defined.");
            }
            ArgumentValidation<T>(master);
            ArgumentValidation<T>(detail);
            ArgumentValidation(processData);
            ArgumentValidation(destinationFolder);
            ArgumentValidation((IFileSource)master);
            ArgumentValidation((IFileSource)detail);
        }

        private static void ArgumentValidation<T>(DelimitedFileSource<T> delimitedFileSource)
        {
            if (string.IsNullOrWhiteSpace(delimitedFileSource.Delimiter))
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { delimitedFileSource.Delimiter }), "The delimiter can not be null or empty.");
            }
        }

        private static void ArgumentValidation<T>(FixedWidthFileSource<T> fixedWidthFileSource)
        {
            if (fixedWidthFileSource.FixedWidths == null)
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { fixedWidthFileSource.FixedWidths }), "The file source fixed widths can not be null.");
            }
            if (fixedWidthFileSource.FixedWidths.Length == 0)
            {
                throw new ArgumentException("The file source fixed widths cannot be empty.", SortHelpers.GetParameterName(new { fixedWidthFileSource.FixedWidths }));
            }
        }

        private static void ArgumentValidation(Action<MergePurgeParam> processData)
        {
            if (processData == null)
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { processData }), "The processData action method must be defined.");
            }
        }

        private static void ArgumentValidation(string destinationFolder)
        {
            if (destinationFolder != null && !Directory.Exists(destinationFolder))
            {
                throw new DirectoryNotFoundException(string.Format("The destination folder, {0} , does not exist.", destinationFolder));
            }
        }

        private static void ArgumentValidation<T>(MasterDelimitedFileSource<T> master, FixedWidthFileSource<T> detail, Action<MergePurgeParam> processData, string destinationFolder = null)
        {
            if ((master.GetKey != null && detail.GetKey == null) || (master.GetKey == null && detail.GetKey != null))
            {
                throw new ArgumentException("Master and Detail must have the same GetKey function type defined.");
            }
            if (master.GetKey == null)
            {
                throw new ArgumentException("The Master File Source must have a GetKey function defined.");
            }
            if (detail.GetKey == null)
            {
                throw new ArgumentException("The Detail File Source must have a GetKey function defined.");
            }
            ArgumentValidation<T>(master);
            ArgumentValidation<T>(detail);
            ArgumentValidation(processData);
            ArgumentValidation(destinationFolder);
            ArgumentValidation((IFileSource)master);
            ArgumentValidation((IFileSource)detail);
        }

        private static void ArgumentValidation<T>(MasterFixedWidthFileSource<T> master, FixedWidthFileSource<T> detail, Action<MergePurgeParam> processData, string destinationFolder = null)
        {
            if ((master.GetKey != null && detail.GetKey == null) || (master.GetKey == null && detail.GetKey != null))
            {
                throw new ArgumentException("Master and Detail must have the same GetKey function type defined.");
            }
            if (master.GetKey == null)
            {
                throw new ArgumentException("The Master File Source must have a GetKey function defined.");
            }
            if (detail.GetKey == null)
            {
                throw new ArgumentException("The Detail File Source must have a GetKey function defined.");
            }
            ArgumentValidation<T>(master);
            ArgumentValidation<T>(detail);
            ArgumentValidation(processData);
            ArgumentValidation(destinationFolder);
            ArgumentValidation((IFileSource)master);
            ArgumentValidation((IFileSource)detail);
        }

        private static void ArgumentValidation<T>(MasterFixedWidthFileSource<T> master, DelimitedFileSource<T> detail, Action<MergePurgeParam> processData, string destinationFolder = null)
        {
            if ((master.GetKey != null && detail.GetKey == null) || (master.GetKey == null && detail.GetKey != null))
            {
                throw new ArgumentException("Master and Detail must have the same GetKey function type defined.");
            }
            if (master.GetKey == null)
            {
                throw new ArgumentException("The Master File Source must have a GetKey function defined.");
            }
            if (detail.GetKey == null)
            {
                throw new ArgumentException("The Detail File Source must have a GetKey function defined.");
            }
            ArgumentValidation<T>(master);
            ArgumentValidation<T>(detail);
            ArgumentValidation(processData);
            ArgumentValidation(destinationFolder);
            ArgumentValidation((IFileSource)master);
            ArgumentValidation((IFileSource)detail);
        }

        private static void ArgumentValidation(IFileSource fileSource)
        {
            if (string.IsNullOrWhiteSpace(fileSource.SourceFilePath))
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { fileSource }), "The sourceFilePath cannot be null or empty.");
            }
            if (!File.Exists(fileSource.SourceFilePath))
            {
                throw new FileNotFoundException(string.Format("The sourceFilePath , {0} , does not exist.", fileSource.SourceFilePath));
            }
        }

    }
}
