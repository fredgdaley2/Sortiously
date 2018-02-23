using System;
using System.IO;
using LiteDB;

namespace DarthSortious
{
    public static class MasterDetail
    {
        public static MergePurgeResults MergePurge(MasterDelimitedFileSource master, DelimitedFileSource detail, Action<MergePurgeParam> processData, string destinationFolder = null, DataMode processMode = DataMode.Passive)
        {
            ArgumentValidation(master, detail, processData, destinationFolder);
            MergePurgeResults mgPurgeResults = new MergePurgeResults();
            SortVars mstSortVars = new SortVars(master.SourceFilePath, destinationFolder);
            SortResults srtResults = SortDelimited(master, mstSortVars.DestFolder);
            mgPurgeResults.InitFilePaths(master.SourceFilePath, detail.SourceFilePath, mstSortVars.DestFolder);
            try
            {
                string hdr = string.Empty;
                using (StreamReader reader = new StreamReader(detail.SourceFilePath))
                using (StreamWriter addSw = new StreamWriter(mgPurgeResults.AddsFilePath))
                using (StreamWriter delSw = new StreamWriter(mgPurgeResults.DeletesFilePath))
                using (StreamWriter updSw = new StreamWriter(mgPurgeResults.UpdatesFilePath))
                using (StreamWriter ignSw = new StreamWriter(mgPurgeResults.IgnoredFilePath))
                using (var db = new LiteDatabase(srtResults.DbConnPath))
                {
                    StreamWriter[] actionWriters = { addSw, delSw, updSw, ignSw };
                    string line;
                    hdr = GetHeader(detail.HasHeader, reader);
                    WriteHeaderToActionWriters(processMode, detail.HasHeader, hdr, actionWriters);
                    LiteCollection<SortKeyNum> colNum = null;
                    LiteCollection<SortKeyString> colStr = null;
                    if (master.GetNumKey != null)
                    {
                        colNum = db.GetCollection<SortKeyNum>(Constants.SortCollectionName);
                    }
                    else
                    {

                        colStr = db.GetCollection<SortKeyString>(Constants.SortCollectionName);
                    }
                    while ((line = reader.ReadLine()) != null)
                    {
                        MergePurgeParam mgPurgeParam = new MergePurgeParam();
                        FileParser.ParseDelimitedString(new StringReader(line), (fields, lNum) =>
                        {
                            mgPurgeParam.DetailFields = fields;
                            mgPurgeParam.DataAction = MergePurgeAction.Ignore;
                        }, detail.Delimiter);
                        DelimitedSortMurgePurge(line, master, detail, colNum, colStr, mgPurgeParam, processData, mgPurgeResults, processMode, actionWriters);
                    }
                }
                mgPurgeResults.ClearSubFilesIfNoCount();
                if (processMode == DataMode.Active)
                {
                    mgPurgeResults.RemoveSubFilesAndFilePaths();
                }
                srtResults.SortedFilePath = mgPurgeResults.NewMasterFilePath;

                if (master.GetNumKey != null)
                {
                    srtResults.LiteDbWriteOutSorted<SortKeyNum>(srtResults.DbConnPath, srtResults.Header, master.SortDirection, master.Delimiter, deleteDb: true);
                }
                else
                {

                    srtResults.LiteDbWriteOutSorted<SortKeyString>(srtResults.DbConnPath, srtResults.Header, master.SortDirection, master.Delimiter, deleteDb: true);
                }
            }
            catch (Exception)
            {
                ExceptionCleanUp(srtResults.DbConnPath, mgPurgeResults);
                throw;
            }

            return mgPurgeResults;
        }



        public static MergePurgeResults MergePurge(MasterDelimitedFileSource master, FixedWidthFileSource detail, Action<MergePurgeParam> processData, string destinationFolder = null, DataMode processMode = DataMode.Passive)
        {
            ArgumentValidation(master, detail, processData, destinationFolder);
            MergePurgeResults mgPurgeResults = new MergePurgeResults();
            SortVars mstSortVars = new SortVars(master.SourceFilePath, destinationFolder);
            SortResults srtResults = SortDelimited(master, mstSortVars.DestFolder);
            mgPurgeResults.InitFilePaths(master.SourceFilePath, detail.SourceFilePath, mstSortVars.DestFolder);
            try
            {
                string hdr = string.Empty;
                using (StreamReader reader = new StreamReader(detail.SourceFilePath))
                using (StreamWriter addSw = new StreamWriter(mgPurgeResults.AddsFilePath))
                using (StreamWriter delSw = new StreamWriter(mgPurgeResults.DeletesFilePath))
                using (StreamWriter updSw = new StreamWriter(mgPurgeResults.UpdatesFilePath))
                using (StreamWriter ignSw = new StreamWriter(mgPurgeResults.IgnoredFilePath))
                using (var db = new LiteDatabase(srtResults.DbConnPath))
                {
                    StreamWriter[] actionWriters = { addSw, delSw, updSw, ignSw };
                    string line;
                    hdr = GetHeader(detail.HasHeader, reader);
                    WriteHeaderToActionWriters(processMode, detail.HasHeader, hdr, actionWriters);
                    LiteCollection<SortKeyNum> colNum = null;
                    LiteCollection<SortKeyString> colStr = null;
                    if (master.GetNumKey != null)
                    {
                        colNum = db.GetCollection<SortKeyNum>(Constants.SortCollectionName);
                    }
                    else
                    {

                        colStr = db.GetCollection<SortKeyString>(Constants.SortCollectionName);
                    }
                    while ((line = reader.ReadLine()) != null)
                    {
                        MergePurgeParam mgPurgeParam = new MergePurgeParam();
                        FileParser.ParseFixedWidthString(new StringReader(line), (fields, lNum) =>
                        {
                            mgPurgeParam.DetailFields = fields;
                            mgPurgeParam.DataAction = MergePurgeAction.Ignore;
                        }, detail.FixedWidths);
                        FixedWidthSortMurgePurge(line, master, detail, colNum, colStr, mgPurgeParam, processData, mgPurgeResults, processMode, actionWriters);
                    }
                }
                mgPurgeResults.ClearSubFilesIfNoCount();
                if (processMode == DataMode.Active)
                {
                    mgPurgeResults.RemoveSubFilesAndFilePaths();
                }
                srtResults.SortedFilePath = mgPurgeResults.NewMasterFilePath;
                if (master.GetNumKey != null)
                {
                    srtResults.LiteDbWriteOutSorted<SortKeyNum>(srtResults.DbConnPath, srtResults.Header, master.SortDirection, master.Delimiter, deleteDb: true);
                }
                else
                {

                    srtResults.LiteDbWriteOutSorted<SortKeyString>(srtResults.DbConnPath, srtResults.Header, master.SortDirection, master.Delimiter, deleteDb: true);
                }
            }
            catch (Exception)
            {
                ExceptionCleanUp(srtResults.DbConnPath, mgPurgeResults);
                throw;
            }

            return mgPurgeResults;
        }

        public static MergePurgeResults MergePurge(MasterFixedWidthFileSource master, FixedWidthFileSource detail, Action<MergePurgeParam> processData, string destinationFolder = null, DataMode processMode = DataMode.Passive)
        {
            ArgumentValidation(master, detail, processData, destinationFolder);
            MergePurgeResults mgPurgeResults = new MergePurgeResults();
            SortVars mstSortVars = new SortVars(master.SourceFilePath, destinationFolder);
            SortResults srtResults = SortFixedWidth(master, mstSortVars.DestFolder);
            mgPurgeResults.InitFilePaths(master.SourceFilePath, detail.SourceFilePath, mstSortVars.DestFolder);
            try
            {
                string hdr = string.Empty;
                using (StreamReader reader = new StreamReader(detail.SourceFilePath))
                using (StreamWriter addSw = new StreamWriter(mgPurgeResults.AddsFilePath))
                using (StreamWriter delSw = new StreamWriter(mgPurgeResults.DeletesFilePath))
                using (StreamWriter updSw = new StreamWriter(mgPurgeResults.UpdatesFilePath))
                using (StreamWriter ignSw = new StreamWriter(mgPurgeResults.IgnoredFilePath))
                using (var db = new LiteDatabase(srtResults.DbConnPath))
                {
                    StreamWriter[] actionWriters = { addSw, delSw, updSw, ignSw };
                    string line;
                    hdr = GetHeader(detail.HasHeader, reader);
                    WriteHeaderToActionWriters(processMode, detail.HasHeader, hdr, actionWriters);
                    LiteCollection<SortKeyNum> colNum = null;
                    LiteCollection<SortKeyString> colStr = null;
                    if (master.GetNumKey != null)
                    {
                        colNum = db.GetCollection<SortKeyNum>(Constants.SortCollectionName);
                    }
                    else
                    {

                        colStr = db.GetCollection<SortKeyString>(Constants.SortCollectionName);
                    }
                    while ((line = reader.ReadLine()) != null)
                    {
                        MergePurgeParam mgPurgeParam = new MergePurgeParam();
                        FileParser.ParseFixedWidthString(new StringReader(line), (fields, lNum) =>
                        {
                            mgPurgeParam.DetailFields = fields;
                            mgPurgeParam.DataAction = MergePurgeAction.Ignore;
                        }, detail.FixedWidths);
                        FixedWidthSortMurgePurge(line, master, detail, colNum, colStr, mgPurgeParam, processData, mgPurgeResults, processMode, actionWriters);
                    }
                }
                mgPurgeResults.ClearSubFilesIfNoCount();
                if (processMode == DataMode.Active)
                {
                    mgPurgeResults.RemoveSubFilesAndFilePaths();
                }
                srtResults.SortedFilePath = mgPurgeResults.NewMasterFilePath;
                if (master.GetNumKey != null)
                {
                    srtResults.LiteDbWriteOutSorted<SortKeyNum>(srtResults.DbConnPath, srtResults.Header, master.SortDirection, Delimiters.Tab, compressed: true, deleteDb: true);
                }
                else
                {

                    srtResults.LiteDbWriteOutSorted<SortKeyString>(srtResults.DbConnPath, srtResults.Header, master.SortDirection, Delimiters.Tab, compressed: true, deleteDb: true);
                }
            }
            catch (Exception)
            {
                ExceptionCleanUp(srtResults.DbConnPath, mgPurgeResults);
                throw;
            }

            return mgPurgeResults;
        }

        public static MergePurgeResults MergePurge(MasterFixedWidthFileSource master, DelimitedFileSource detail, Action<MergePurgeParam> processData, string destinationFolder = null, DataMode processMode = DataMode.Passive)
        {
            ArgumentValidation(master, detail, processData, destinationFolder);
            MergePurgeResults mgPurgeResults = new MergePurgeResults();
            SortVars mstSortVars = new SortVars(master.SourceFilePath, destinationFolder);
            SortResults srtResults = SortFixedWidth(master, mstSortVars.DestFolder);
            mgPurgeResults.InitFilePaths(master.SourceFilePath, detail.SourceFilePath, mstSortVars.DestFolder);
            try
            {
                string hdr = string.Empty;
                using (StreamReader reader = new StreamReader(detail.SourceFilePath))
                using (StreamWriter addSw = new StreamWriter(mgPurgeResults.AddsFilePath))
                using (StreamWriter delSw = new StreamWriter(mgPurgeResults.DeletesFilePath))
                using (StreamWriter updSw = new StreamWriter(mgPurgeResults.UpdatesFilePath))
                using (StreamWriter ignSw = new StreamWriter(mgPurgeResults.IgnoredFilePath))
                using (var db = new LiteDatabase(srtResults.DbConnPath))
                {
                    StreamWriter[] actionWriters = { addSw, delSw, updSw, ignSw };
                    string line;
                    hdr = GetHeader(detail.HasHeader, reader);
                    WriteHeaderToActionWriters(processMode, detail.HasHeader, hdr, actionWriters);
                    LiteCollection<SortKeyNum> colNum = null;
                    LiteCollection<SortKeyString> colStr = null;
                    if (master.GetNumKey != null)
                    {
                        colNum = db.GetCollection<SortKeyNum>(Constants.SortCollectionName);
                    }
                    else
                    {

                        colStr = db.GetCollection<SortKeyString>(Constants.SortCollectionName);
                    }
                    while ((line = reader.ReadLine()) != null)
                    {
                        MergePurgeParam mgPurgeParam = new MergePurgeParam();
                        FileParser.ParseDelimitedString(new StringReader(line), (fields, lNum) =>
                        {
                            mgPurgeParam.DetailFields = fields;
                            mgPurgeParam.DataAction = MergePurgeAction.Ignore;
                        }, detail.Delimiter);
                        FixedWidthSortMurgePurge(line, master, detail, colNum, colStr, mgPurgeParam, processData, mgPurgeResults, processMode, actionWriters);
                    }
                }
                mgPurgeResults.ClearSubFilesIfNoCount();
                if (processMode == DataMode.Active)
                {
                    mgPurgeResults.RemoveSubFilesAndFilePaths();
                }
                srtResults.SortedFilePath = mgPurgeResults.NewMasterFilePath;
                if (master.GetNumKey != null)
                {
                    srtResults.LiteDbWriteOutSorted<SortKeyNum>(srtResults.DbConnPath, srtResults.Header, master.SortDirection, Delimiters.Tab, compressed: true, deleteDb: true);
                }
                else
                {

                    srtResults.LiteDbWriteOutSorted<SortKeyString>(srtResults.DbConnPath, srtResults.Header, master.SortDirection, Delimiters.Tab, compressed: true, deleteDb: true);
                }
            }
            catch (Exception)
            {
                ExceptionCleanUp(srtResults.DbConnPath, mgPurgeResults);
                throw;
            }

            return mgPurgeResults;
        }



        internal static SortResults SortDelimited(MasterDelimitedFileSource master, string destinationFolder)
        {
            SortResults srtResults;
            if (master.GetNumKey != null)
            {
                srtResults = LiteDbSortFile.SortDelimitedByKeyCore<long>(sourcefilePath: master.SourceFilePath,
                    getKey: master.GetNumKey,
                    destinationFolder: destinationFolder,
                    delimiter: master.Delimiter,
                    hasHeader: master.HasHeader,
                    isUniqueKey: false,
                    sortDir: master.SortDirection,
                    deleteDbConnPath: false,
                    writeOutSortFile: false);
            }
            else
            {
                srtResults = LiteDbSortFile.SortDelimitedByKeyCore<string>(sourcefilePath: master.SourceFilePath,
                    getKey: master.GetStringKey,
                    destinationFolder: destinationFolder,
                    delimiter: master.Delimiter,
                    hasHeader: master.HasHeader,
                    isUniqueKey: false,
                    sortDir: master.SortDirection,
                    deleteDbConnPath: false,
                    writeOutSortFile: false);
            }

            return srtResults;
        }

        internal static SortResults SortFixedWidth(MasterFixedWidthFileSource master, string destinationFolder)
        {
            SortResults srtResults;
            if (master.GetNumKey != null)
            {
                srtResults = LiteDbSortFile.SortFixedWidthByKeyCore<long>(sourcefilePath: master.SourceFilePath,
                                       getKey: master.GetNumKey,
                                       destinationFolder: destinationFolder,
                                       hasHeader: master.HasHeader,
                                       isUniqueKey: false,
                                       sortDir: master.SortDirection,
                                       deleteDbConnPath: false,
                                       writeOutSortFile: false);
            }
            else
            {
                srtResults = LiteDbSortFile.SortFixedWidthByKeyCore<string>(sourcefilePath: master.SourceFilePath,
                     getKey: master.GetStringKey,
                     destinationFolder: destinationFolder,
                     hasHeader: master.HasHeader,
                     isUniqueKey: false,
                     sortDir: master.SortDirection,
                     deleteDbConnPath: false,
                     writeOutSortFile: false);
            }

            return srtResults;
        }


        private static void DelimitedSortMurgePurge(string line,
            MasterDelimitedFileSource master,
            DelimitedFileSource detail,
            LiteCollection<SortKeyNum> colNum,
            LiteCollection<SortKeyString> colStr,
            MergePurgeParam mgPurgeParam,
            Action<MergePurgeParam> processData,
            MergePurgeResults mgPurgeResults,
            DataMode processMode,
            StreamWriter[] actionWriters)
        {
            dynamic key = null;
            SortKeyNum srtKeyNum = null;
            SortKeyString srtKeyString = null;
            string masterData = string.Empty;
            if (colNum != null)
            {
                key = detail.GetNumKey(mgPurgeParam.DetailFields, line);
                srtKeyNum = GetSortKeyNumFromCollection(colNum, key);
                mgPurgeParam.KeyFound = KeyFound(srtKeyNum);
                masterData = GetSortKeyNumData(srtKeyNum);
            }
            else
            {
                key = detail.GetStringKey(mgPurgeParam.DetailFields, line);
                srtKeyString = GetSortKeyStringFromCollection(colStr, key);
                mgPurgeParam.KeyFound = KeyFound(srtKeyString);
                masterData = GetSortKeyStringData(srtKeyString);
            }

            if (mgPurgeParam.KeyFound)
            {
                mgPurgeParam.MasterFields = GetMasterFields(master, masterData);
            }
            if (processData != null)
            {
                processData(mgPurgeParam);
                mgPurgeResults.IncrementAction(mpAction: mgPurgeParam.DataAction);
                string detailLine = GetDetailLine(detail, mgPurgeParam);
                string newMasterData = GetNewMasterDelimitedData(master, GetMasterLine(master, mgPurgeParam));
                PerformDataActionForPassiveModeWriter(mgPurgeParam.DataAction, mgPurgeParam.KeyFound, processMode, detailLine, actionWriters);
                PerformDataAction(mgPurgeParam, processMode, newMasterData, key, colNum, colStr, srtKeyNum, srtKeyString);

            }
        }

        private static void FixedWidthSortMurgePurge(string line,
            MasterDelimitedFileSource master,
            FixedWidthFileSource detail,
            LiteCollection<SortKeyNum> colNum,
            LiteCollection<SortKeyString> colStr,
            MergePurgeParam mgPurgeParam,
            Action<MergePurgeParam> processData,
            MergePurgeResults mgPurgeResults,
            DataMode processMode,
            StreamWriter[] actionWriters)
        {
            dynamic key = null;
            SortKeyNum srtKeyNum = null;
            SortKeyString srtKeyString = null;
            string masterData = string.Empty;
            if (colNum != null)
            {
                key = detail.GetNumKey(line);
                srtKeyNum = GetSortKeyNumFromCollection(colNum, key);
                mgPurgeParam.KeyFound = KeyFound(srtKeyNum);
                masterData = GetSortKeyNumData(srtKeyNum);
            }
            else
            {
                key = detail.GetStringKey(line);
                srtKeyString = GetSortKeyStringFromCollection(colStr, key);
                mgPurgeParam.KeyFound = KeyFound(srtKeyString);
                masterData = GetSortKeyStringData(srtKeyString);
            }

            if (mgPurgeParam.KeyFound)
            {
                mgPurgeParam.MasterFields = GetMasterFields(master, masterData);
            }
            if (processData != null)
            {
                processData(mgPurgeParam);
                mgPurgeResults.IncrementAction(mpAction: mgPurgeParam.DataAction);
                string detailLine = GetDetailLine(detail, mgPurgeParam);
                string newMasterData = GetNewMasterDelimitedData(master, GetMasterLine(master, mgPurgeParam));
                PerformDataActionForPassiveModeWriter(mgPurgeParam.DataAction, mgPurgeParam.KeyFound, processMode, detailLine, actionWriters);
                PerformDataAction(mgPurgeParam, processMode, newMasterData, key, colNum, colStr, srtKeyNum, srtKeyString);
            }
        }


        private static void FixedWidthSortMurgePurge(string line,
            MasterFixedWidthFileSource master,
            FixedWidthFileSource detail,
            LiteCollection<SortKeyNum> colNum,
            LiteCollection<SortKeyString> colStr,
            MergePurgeParam mgPurgeParam,
            Action<MergePurgeParam> processData,
            MergePurgeResults mgPurgeResults,
            DataMode processMode,
            StreamWriter[] actionWriters)
        {

            dynamic key = null;
            SortKeyNum srtKeyNum = null;
            SortKeyString srtKeyString = null;
            string masterData = string.Empty;
            if (colNum != null)
            {
                key = detail.GetNumKey(line);
                srtKeyNum = GetSortKeyNumFromCollection(colNum, key);
                mgPurgeParam.KeyFound = KeyFound(srtKeyNum);
                masterData = GetSortKeyNumData(srtKeyNum);
            }
            else
            {
                key = detail.GetStringKey(line);
                srtKeyString = GetSortKeyStringFromCollection(colStr, key);
                mgPurgeParam.KeyFound = KeyFound(srtKeyString);
                masterData = GetSortKeyStringData(srtKeyString);
            }

            if (mgPurgeParam.KeyFound)
            {
                mgPurgeParam.MasterFields = GetMasterFields(master, masterData);
            }

            if (processData != null)
            {
                processData(mgPurgeParam);
                mgPurgeResults.IncrementAction(mpAction: mgPurgeParam.DataAction);
                string detailLine = GetDetailLine(detail, mgPurgeParam);
                string newMasterData = GetNewMasterDataFixedWidth(GetMasterLine(master, mgPurgeParam));
                PerformDataActionForPassiveModeWriter(mgPurgeParam.DataAction, mgPurgeParam.KeyFound, processMode, detailLine, actionWriters);
                PerformDataAction(mgPurgeParam, processMode, newMasterData, key, colNum, colStr, srtKeyNum, srtKeyString);
            }
        }

        private static void FixedWidthSortMurgePurge(string line,
            MasterFixedWidthFileSource master,
            DelimitedFileSource detail,
            LiteCollection<SortKeyNum> colNum,
            LiteCollection<SortKeyString> colStr,
            MergePurgeParam mgPurgeParam,
            Action<MergePurgeParam> processData,
            MergePurgeResults mgPurgeResults,
            DataMode processMode,
            StreamWriter[] actionWriters)
        {
            dynamic key = null;
            SortKeyNum srtKeyNum = null;
            SortKeyString srtKeyString = null;
            string masterData = string.Empty;
            if (colNum != null)
            {
                key = detail.GetNumKey(mgPurgeParam.DetailFields, line);
                srtKeyNum = GetSortKeyNumFromCollection(colNum, key);
                mgPurgeParam.KeyFound = KeyFound(srtKeyNum);
                masterData = GetSortKeyNumData(srtKeyNum);
            }
            else
            {
                key = detail.GetStringKey(mgPurgeParam.DetailFields, line);
                srtKeyString = GetSortKeyStringFromCollection(colStr, key);
                mgPurgeParam.KeyFound = KeyFound(srtKeyString);
                masterData = GetSortKeyStringData(srtKeyString);
            }

            if (mgPurgeParam.KeyFound)
            {
                mgPurgeParam.MasterFields = GetMasterFields(master, masterData);
            }
            if (processData != null)
            {
                processData(mgPurgeParam);
                mgPurgeResults.IncrementAction(mpAction: mgPurgeParam.DataAction);
                string detailLine = GetDetailLine(detail, mgPurgeParam);
                string newMasterData = GetNewMasterDataFixedWidth(GetMasterLine(master, mgPurgeParam));
                PerformDataActionForPassiveModeWriter(mgPurgeParam.DataAction, mgPurgeParam.KeyFound, processMode, detailLine, actionWriters);
                PerformDataAction(mgPurgeParam, processMode, newMasterData, key, colNum, colStr, srtKeyNum, srtKeyString);
            }
        }


        private static void PerformDataActionForPassiveModeWriter(MergePurgeAction dataAction, bool keyFound, DataMode processMode, string detailLine, StreamWriter[] actionWriters)
        {
            switch (dataAction)
            {
                case MergePurgeAction.Add:
                    if (!keyFound)
                    {
                        PassiveModeWriter(processMode, actionWriters[(int)ActionWriter.AddsWriter], detailLine);
                    }
                    break;
                case MergePurgeAction.Delete:
                    if (keyFound)
                    {
                        PassiveModeWriter(processMode, actionWriters[(int)ActionWriter.DeletesWriter], detailLine);
                    }
                    break;
                case MergePurgeAction.Update:
                    if (keyFound)
                    {
                        PassiveModeWriter(processMode, actionWriters[(int)ActionWriter.UpdatesWriter], detailLine);
                    }

                    break;
                default:
                    PassiveModeWriter(processMode, actionWriters[(int)ActionWriter.IgnoredWriter], detailLine);
                    break;
            }

        }
        private static void PerformDataAction(MergePurgeParam mgPurgeParam,
            DataMode processMode,
            string newMasterData,
            dynamic key,
            LiteCollection<SortKeyNum> colNum,
            LiteCollection<SortKeyString> colStr,
            SortKeyNum srtKeyNum,
            SortKeyString srtKeyString)
        {
            switch (mgPurgeParam.DataAction)
            {
                case MergePurgeAction.Add:
                    if (!mgPurgeParam.KeyFound)
                    {
                        if (processMode == DataMode.Active)
                        {

                            if (colNum != null)
                            {
                                colNum.Insert(new SortKeyNum
                                {
                                    Data = newMasterData,
                                    Key = key
                                });
                            }
                            else
                            {
                                colStr.Insert(new SortKeyString
                                {
                                    Data = newMasterData,
                                    Key = key
                                });
                            }
                        }
                    }
                    break;
                case MergePurgeAction.Delete:
                    if (mgPurgeParam.KeyFound)
                    {
                        if (processMode == DataMode.Active)
                        {
                            if (colNum != null)
                            {
                                colNum.Delete(x => x.Id == srtKeyNum.Id);

                            }
                            else
                            {
                                colStr.Delete(x => x.Id == srtKeyString.Id);
                            }
                        }
                    }
                    break;
                case MergePurgeAction.Update:
                    if (mgPurgeParam.KeyFound)
                    {
                        if (processMode == DataMode.Active)
                        {
                            if (colNum != null)
                            {
                                srtKeyNum.Data = newMasterData;
                                colNum.Update(srtKeyNum);
                            }
                            else
                            {
                                srtKeyString.Data = newMasterData;
                                colStr.Update(srtKeyString);
                            }
                        }
                    }

                    break;
                default:
                    break;
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

        private static SortKeyNum GetSortKeyNumFromCollection(LiteCollection<SortKeyNum> sortKeyNumCollection, long theKey)
        {
            return sortKeyNumCollection.FindOne(x => x.Key == theKey);
        }

        private static SortKeyString GetSortKeyStringFromCollection(LiteCollection<SortKeyString> sortKeyStringCollection, string theKey)
        {
            return sortKeyStringCollection.FindOne(x => x.Key == theKey);
        }

        private static string[] GetMasterFields(MasterDelimitedFileSource master, string data)
        {
            string outLine = SortFileHelpers.UnEscapeByDelimiter(data, master.Delimiter);
            string[] results = null;
            FileParser.ParseDelimitedString(new StringReader(outLine), (fields, lNum) =>
            {
                results = fields;
            }, master.Delimiter);
            return results;
        }

        private static string[] GetMasterFields(MasterFixedWidthFileSource master, string data)
        {
            string outLine = SortFileHelpers.UnEscapeByDelimiter(data.Decompress(), Delimiters.Tab);
            string[] results = null;
            FileParser.ParseFixedWidthString(new StringReader(outLine), (fields, lNum) =>
            {
                results = fields;
            }, master.FixedWidths);
            return results;
        }

        private static string GetMasterLine(MasterFixedWidthFileSource master, MergePurgeParam mgPurgeParam)
        {
            return mgPurgeParam.MasterFields != null ? mgPurgeParam.MasterFields.SerializeToFixedWidth(master.FixedWidths) : string.Empty;
        }

        private static string GetMasterLine(MasterDelimitedFileSource master, MergePurgeParam mgPurgeParam)
        {
            return mgPurgeParam.MasterFields != null ? mgPurgeParam.MasterFields.SerializeToDelimited(master.Delimiter) : string.Empty;
        }

        private static string GetDetailLine(DelimitedFileSource detail, MergePurgeParam mgPurgeParam)
        {
            return mgPurgeParam.DetailFields.SerializeToDelimited(detail.Delimiter);
        }

        private static string GetDetailLine(FixedWidthFileSource detail, MergePurgeParam mgPurgeParam)
        {
            return mgPurgeParam.DetailFields.SerializeToFixedWidth(detail.FixedWidths);
        }

        private static string GetNewMasterDelimitedData(MasterDelimitedFileSource master, string masterLine)
        {
            return masterLine + SortFileHelpers.EscapeByDelimiter(master.Delimiter);
        }

        private static string GetNewMasterDataFixedWidth(string masterLine)
        {
            return (masterLine + Constants.PreserveCharacter).Compress();
        }
        private static void ExceptionCleanUp(string dbConnPath, MergePurgeResults mgPurgeResults)
        {
            SortFileHelpers.DeleteFileIfExists(dbConnPath);
            SortFileHelpers.DeleteFileIfExists(Path.Combine(Path.GetDirectoryName(dbConnPath), SortFileHelpers.GetDbJournalName(dbConnPath)));
            mgPurgeResults.DeleteActionFiles();
            SortFileHelpers.DeleteFileIfExists(mgPurgeResults.NewMasterFilePath);
        }

        private static bool KeyFound(SortKeyString srtKeyString)
        {
            return srtKeyString != null;
        }

        private static bool KeyFound(SortKeyNum srtKeyNum)
        {
            return srtKeyNum != null;
        }

        private static string GetSortKeyStringData(SortKeyString srtKeyString)
        {
            return srtKeyString != null ? srtKeyString.Data : string.Empty;
        }

        private static string GetSortKeyNumData(SortKeyNum srtKeyNum)
        {
            return srtKeyNum != null ? srtKeyNum.Data : string.Empty;
        }

        private static void ArgumentValidation(MasterDelimitedFileSource master, DelimitedFileSource detail, Action<MergePurgeParam> processData, string destinationFolder = null)
        {
            if ((master.GetNumKey != null && detail.GetNumKey == null) || (master.GetNumKey == null && detail.GetNumKey != null) ||
                (master.GetStringKey != null && detail.GetStringKey == null) || (master.GetStringKey == null && detail.GetStringKey != null))
            {
                throw new ArgumentException("Master and Detail must have the same GetKey function type defined.");
            }

            if (master.GetNumKey == null && master.GetStringKey == null)
            {
                throw new ArgumentException("The Master File Source must have a GetKey function defined.");
            }
            if (detail.GetNumKey == null && detail.GetStringKey == null)
            {
                throw new ArgumentException("The Detail File Source must have a GetKey function defined.");
            }
            ArgumentValidation(master);
            ArgumentValidation(detail);
            ArgumentValidation(processData);
            ArgumentValidation(destinationFolder);
            ArgumentValidation((IFileSource)master);
            ArgumentValidation((IFileSource)detail);
        }

        private static void ArgumentValidation(DelimitedFileSource delimitedFileSource)
        {
            if (string.IsNullOrWhiteSpace(delimitedFileSource.Delimiter))
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { delimitedFileSource.Delimiter }), "The delimiter can not be null or empty.");
            }
        }

        private static void ArgumentValidation(FixedWidthFileSource fixedWidthFileSource)
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

        private static void ArgumentValidation(MasterDelimitedFileSource master, FixedWidthFileSource detail, Action<MergePurgeParam> processData, string destinationFolder = null)
        {
            if ((master.GetNumKey != null && detail.GetNumKey == null) || (master.GetNumKey == null && detail.GetNumKey != null) ||
                (master.GetStringKey != null && detail.GetStringKey == null) || (master.GetStringKey == null && detail.GetStringKey != null))
            {
                throw new ArgumentException("Master and Detail must have the same GetKey function type defined.");
            }
            if (master.GetNumKey == null && master.GetStringKey == null)
            {
                throw new ArgumentException("The Master File Source must have a GetKey function defined.");
            }
            if (detail.GetNumKey == null && detail.GetStringKey == null)
            {
                throw new ArgumentException("The Detail File Source must have a GetKey function defined.");
            }
            ArgumentValidation(master);
            ArgumentValidation(detail);
            ArgumentValidation(processData);
            ArgumentValidation(destinationFolder);
            ArgumentValidation((IFileSource)master);
            ArgumentValidation((IFileSource)detail);
        }

        private static void ArgumentValidation(MasterFixedWidthFileSource master, FixedWidthFileSource detail, Action<MergePurgeParam> processData, string destinationFolder = null)
        {
            if ((master.GetNumKey != null && detail.GetNumKey == null) || (master.GetNumKey == null && detail.GetNumKey != null) ||
                (master.GetStringKey != null && detail.GetStringKey == null) || (master.GetStringKey == null && detail.GetStringKey != null))
            {
                throw new ArgumentException("Master and Detail must have the same GetKey function type defined.");
            }
            if (master.GetNumKey == null && master.GetStringKey == null)
            {
                throw new ArgumentException("The Master File Source must have a GetKey function defined.");
            }
            if (detail.GetNumKey == null && detail.GetStringKey == null)
            {
                throw new ArgumentException("The Detail File Source must have a GetKey function defined.");
            }
            ArgumentValidation(master);
            ArgumentValidation(detail);
            ArgumentValidation(processData);
            ArgumentValidation(destinationFolder);
            ArgumentValidation((IFileSource)master);
            ArgumentValidation((IFileSource)detail);
        }

        private static void ArgumentValidation(MasterFixedWidthFileSource master, DelimitedFileSource detail, Action<MergePurgeParam> processData, string destinationFolder = null)
        {
            if ((master.GetNumKey != null && detail.GetNumKey == null) || (master.GetNumKey == null && detail.GetNumKey != null) ||
                (master.GetStringKey != null && detail.GetStringKey == null) || (master.GetStringKey == null && detail.GetStringKey != null))
            {
                throw new ArgumentException("Master and Detail must have the same GetKey function type defined.");
            }
            if (master.GetNumKey == null && master.GetStringKey == null)
            {
                throw new ArgumentException("The Master File Source must have a GetKey function defined.");
            }
            if (detail.GetNumKey == null && detail.GetStringKey == null)
            {
                throw new ArgumentException("The Detail File Source must have a GetKey function defined.");
            }
            ArgumentValidation(master);
            ArgumentValidation(detail);
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
