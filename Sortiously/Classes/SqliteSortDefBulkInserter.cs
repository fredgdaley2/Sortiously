using System;
using System.Collections.Generic;
using System.Data.SQLite;


namespace Sortiously
{
    public class SqliteSortDefBulkInserter : IDisposable
    {
        private readonly int MaxBatchSize;
        private bool disposed;
        private readonly SortDefinitions sortDefs;
        private readonly SQLiteConnection dbConnection;
        private readonly List<SortKeyData> sortKeyDataList;
        private string insertBulkCmd;

        public SqliteSortDefBulkInserter(string connStr, SortDefinitions sortDefinitions, int maxBatchSize = 250000)
        {
            sortDefs = sortDefinitions;
            MaxBatchSize = maxBatchSize;
            sortKeyDataList = new List<SortKeyData>();
            SQLiteConnectionStringBuilder connBldr = new SQLiteConnectionStringBuilder();
            connBldr.DataSource = connStr;
            connBldr.JournalMode = SQLiteJournalModeEnum.Off;
            connBldr.Version = 3;
            dbConnection = new SQLiteConnection(connBldr.ConnectionString);
            dbConnection.Open();
            CreateTable();
            SetInsertBulkCommand();
        }

        private void CreateTable()
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

        private string GetCreateKeyTableCmd()
        {
            return string.Format(@"CREATE TABLE FileData ( Id Integer primary key autoincrement, {0} , LineData TEXT NOT NULL);", sortDefs.BuildTableDefinitions());
        }

        private string GetCreateKeyIndexCmd()
        {
            return string.Format(@"CREATE INDEX id_idx ON FileData (Id);{0}", sortDefs.BuildIndexDefinitions());
            //return string.Format(@"CREATE INDEX sortKey_idx ON FileData (SortKey {0});CREATE INDEX id_idx ON FileData (Id);", sortDirection == SortDirection.Ascending ? "ASC" : "DESC");
        }

        private void SqliteExecuteNonQuery(string sqlCmd)
        {
            using (var cmd = new SQLiteCommand(sqlCmd, dbConnection))
            {
                cmd.ExecuteNonQuery();
            }
        }


        internal void Add(SortKeyData sortData)
        {
            sortKeyDataList.Add(sortData);
            InsertIfCountMatchesMax();

        }

        private void InsertIfCountMatchesMax()
        {
            if (sortKeyDataList.Count == MaxBatchSize)
            {
                InsertBulk();
            }
        }

        internal void InsertAnyLeftOvers()
        {
            if (sortKeyDataList.Count > 0)
            {
                InsertBulk();
            }
        }

        private void SetInsertBulkCommand()
        {
            insertBulkCmd = string.Format(@"INSERT INTO FileData ({0},LineData) VALUES ({1},@data);", sortDefs.BuildInsertColumns(), sortDefs.BuildInsertParamters());
        }

        private void InsertBulk()
        {

            int numberOfKeys = sortDefs.GetKeys().Count;
            List<SortDefinition> sortKeys  = sortDefs.GetKeys();

            using (var cmd = new SQLiteCommand(dbConnection))
            {

                using (var transaction = dbConnection.BeginTransaction())
                {

                    cmd.CommandText = insertBulkCmd;


                    for (int pvdx = 0; pvdx < numberOfKeys; pvdx++)
                    {
                        cmd.Parameters.AddWithValue(string.Format("@key{0}", pvdx), string.Format("SortKey{0}", pvdx));
                    }
                    cmd.Parameters.AddWithValue("@data", "LineData");

                    foreach (var item in sortKeyDataList)
                    {

                        for (int i = 0; i < numberOfKeys; i++)
                        {
                            if (sortKeys[i].DataType == KeyType.Numberic)
                            {
                                cmd.Parameters[string.Format("@key{0}", i)].Value = Convert.ToInt64(item.KeyValues[i]);
                            }
                            else
                            {
                                cmd.Parameters[string.Format("@key{0}", i)].Value = Convert.ToString(item.KeyValues[i]);
                            }
                        }
                        cmd.Parameters["@data"].Value = item.Data;
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
            sortKeyDataList.Clear();
        }

        internal void AddUnUniqueIndex()
        {
            SqliteExecuteNonQuery(GetCreateKeyIndexCmd());
        }


    }
}
