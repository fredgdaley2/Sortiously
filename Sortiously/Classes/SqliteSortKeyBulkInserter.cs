using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Sortiously
{
    internal class SqliteSortKeyBulkInserter<T> : IDisposable
    {
        private readonly int MaxBatchSize;
        private bool disposed;
        private readonly bool hasUniqueKey;
        private readonly SortDirection sortDirection;
        private readonly SQLiteConnection dbConnection;
        private List<SortKey<T>> SortKeyList { get; }

        public SqliteSortKeyBulkInserter(string connStr, SortDirection sortDir = SortDirection.Ascending, bool uniqueKey = false, int maxBatchSize = 250000)
        {
            SortKeyList = new List<SortKey<T>>();
            hasUniqueKey = uniqueKey;
            sortDirection = sortDir;
            dbConnection = new SQLiteConnection(@"Data Source=" + connStr);
            MaxBatchSize = maxBatchSize;
            dbConnection.Open();
            CreateStringNumTable();

        }

        private void CreateStringNumTable()
        {

            SqliteExecuteNonQuery(GetCreateKeyTableCmd());

        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            disposed = true;
        }

        private static string GetCreateKeyTableCmd()
        {
            if (typeof(T) == typeof(long))
            {
                return @"CREATE TABLE FileData ( Id Integer primary key autoincrement, SortKey Integer NOT NULL, LineData TEXT NOT NULL);";
            }
            return @"CREATE TABLE FileData ( Id Integer primary key autoincrement, SortKey TEXT NOT NULL, LineData TEXT NOT NULL);";
        }

        private string GetCreateKeyIndexCmd()
        {
            return string.Format(@"CREATE INDEX sortKey_idx ON FileData (SortKey {0});CREATE INDEX id_idx ON FileData (Id);", sortDirection == SortDirection.Ascending ? "ASC" : "DESC");
        }

        private void SqliteExecuteNonQuery(string sqlCmd)
        {
            using (var cmd = new SQLiteCommand(sqlCmd, dbConnection))
            {
                cmd.ExecuteNonQuery();
            }
        }


        public string Add(T theKey, string theData)
        {
            string dupeLine = string.Empty;
            SortKeyList.Add(new SortKey<T>
            {
                Key = theKey,
                Data = theData
            });
            InsertIfCountMatchesMax();
            return dupeLine;

        }

        private void InsertIfCountMatchesMax()
        {
            if (SortKeyList.Count == MaxBatchSize)
            {
                InsertBulk();
            }
        }

        public void InsertAnyLeftOvers()
        {
            if (SortKeyList.Count > 0)
            {
                InsertBulk();
            }
        }

        private void InsertBulk()
        {

            const string sqlInsertUsers =
            @"INSERT INTO FileData (SortKey,LineData) VALUES (@key,@data);";
            using (var cmd = new SQLiteCommand(dbConnection))
            {

                using (var transaction = dbConnection.BeginTransaction())
                {

                    cmd.CommandText = sqlInsertUsers;
                    cmd.Parameters.AddWithValue("@key", "SortKey");
                    cmd.Parameters.AddWithValue("@data", "LineData");
                    foreach (var item in SortKeyList)
                    {
                        cmd.Parameters["@key"].Value = item.Key;
                        cmd.Parameters["@data"].Value = item.Data;
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
            SortKeyList.Clear();
       }


        public void AddUnUniqueIndex()
        {
            SqliteExecuteNonQuery(GetCreateKeyIndexCmd());
        }

    }

}
