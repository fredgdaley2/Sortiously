using System;
using System.Collections.Generic;
using LiteDB;

namespace DarthSortious
{
    public class SortKeyStringBulkInserter : IDisposable
    {
        const int MaxBatchSize = 5000;
        bool disposed;
        bool hasUniqueKey;
        private List<SortKeyString> SortKeyStringList { get; set; }
        private LiteDatabase SortDb { get; set; }
        private LiteCollection<SortKeyString> SortKeyStringCollection { get; set; }

        public SortKeyStringBulkInserter(string connStr, string collectionName = Constants.SortCollectionName , bool uniqueKey = false)
        {
            SortKeyStringList = new List<SortKeyString>();
            SortDb = new LiteDatabase(connStr);
            SortKeyStringCollection = SortDb.GetCollection<SortKeyString>(collectionName);
            SortKeyStringCollection.EnsureIndex(x => x.Key, uniqueKey);
            hasUniqueKey = uniqueKey;

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
                SortDb.Dispose();
            }

            disposed = true;
        }

        public string Add(string theKey, string theData)
        {
            string dupeLine = string.Empty;
            if (!hasUniqueKey || hasUniqueKey && !KeyExists(theKey))
            {
                SortKeyStringList.Add(new SortKeyString
                {
                    Key = theKey,
                    Data = theData
                });
                InsertIfCountMatchesMax();
            }
            else
            {
                dupeLine = theData;
            }
            return dupeLine;
        }

        private void InsertIfCountMatchesMax()
        {
            if (SortKeyStringList.Count == MaxBatchSize)
            {
                InsertBulk();
            }
        }

        public void InsertAnyLeftOvers()
        {
            if (SortKeyStringList.Count > 0)
            {
                InsertBulk();
            }
        }

        private void InsertBulk()
        {
            SortKeyStringCollection.InsertBulk(SortKeyStringList);
            SortKeyStringList.Clear();
        }

        private bool KeyExists(string theKey)
        {
            bool kExists = SortKeyStringCollection.FindOne(x => x.Key == theKey) != null;
            if (!kExists)
            {
                kExists = SortKeyStringList.Exists(x => x.Key == theKey);
            }
            return kExists;


        }
    }
}
