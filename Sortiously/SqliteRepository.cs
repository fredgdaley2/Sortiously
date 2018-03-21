using System;
using System.Data.SQLite;

namespace Sortiously
{
    public class SqliteRepository<T> : IDisposable
    {
        bool disposed;
        private readonly SQLiteConnection dbConnection;

        public SqliteRepository(string connStr)
        {
            dbConnection = new SQLiteConnection(@"Data Source=" + connStr);
            dbConnection.Open();
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


        public void Add(T theKey, string theData)
        {
            const string sqlInsert =
            @"INSERT INTO FileData (SortKey,LineData) VALUES (@key,@data);";
            using (var cmd = new SQLiteCommand(dbConnection))
            {

                using (var transaction = dbConnection.BeginTransaction())
                {

                    cmd.CommandText = sqlInsert;
                    cmd.Parameters.AddWithValue("@key", "SortKey");
                    cmd.Parameters.AddWithValue("@data", "LineData");
                    cmd.Parameters["@key"].Value = theKey;
                    cmd.Parameters["@data"].Value = theData;
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }


        public void Update(long id, string theData)
        {
            string sqlUpdate =
            string.Format(@"UPDATE FileData SET LineData = '{0}' WHERE Id = {1};", theData, id);
            using (var cmd = new SQLiteCommand(dbConnection))
            {

                using (var transaction = dbConnection.BeginTransaction())
                {

                    cmd.CommandText = sqlUpdate;
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }


        public void Delete(long id)
        {
            using (var cmd = new SQLiteCommand(@"Delete FROM FileData WHERE Id = " + id, dbConnection))
            {

                using (var transaction = dbConnection.BeginTransaction())
                {
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }


        public SortKey<T> KeyInDb(T theKey)
        {
            string sqlCmd = null;
            if (typeof(T) == typeof(long))
            {
                sqlCmd = @"SELECT * FROM FileData WHERE SortKey = " + theKey;
            }
            else
            {
                sqlCmd = string.Format(@"SELECT * FROM FileData WHERE SortKey = '{0}'", theKey);

            }

            using (var cmd = new SQLiteCommand(sqlCmd, dbConnection))
            {
                SortKey<T> srtKey = null;
                using (SQLiteDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    if (rdr.Read())
                    {
                        var tmpId = rdr["Id"];
                        dynamic lastReadKey = null;
                        long id = (long)tmpId;
                        string sqlLiteData = (string)rdr["LineData"];
                        lastReadKey = rdr["SortKey"];
                        srtKey = new SortKey<T>() { Id = id, Data = sqlLiteData, Key = lastReadKey, Found = true };

                    }
                }
                if (srtKey == null)
                {
                    srtKey = new SortKey<T>() { Id = 0, Data = string.Empty, Key = theKey , Found = false };
                }
                return srtKey;
            }
        }

    }

}
