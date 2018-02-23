using System;
using System.Collections.Generic;
using LiteDB;

namespace DarthSortious
{
    public class SortKeyNumBulkInserter : IDisposable
    {
        const int MaxBatchSize = 5000;
        bool disposed;
        bool hasUniqueKey;
        public List<SortKeyNum> SortKeyNumList { get; set; }

        public LiteDatabase SortDb { get; set; }

        public LiteCollection<SortKeyNum> SortKeyNumCollection { get; set; }

        public SortKeyNumBulkInserter(string connStr, string collectionName = Constants.SortCollectionName, bool uniqueKey = false)
        {
            SortKeyNumList = new List<SortKeyNum>();
            SortDb = new LiteDatabase(connStr);
            SortKeyNumCollection = SortDb.GetCollection<SortKeyNum>(collectionName);
            SortKeyNumCollection.EnsureIndex(x => x.Key, uniqueKey);
            hasUniqueKey = uniqueKey;

        }

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                SortDb.Dispose();
            }

            disposed = true;
        }

        public string Add(long theKey, string theData)
        {
            string dupeLine = string.Empty;
            if (!hasUniqueKey || hasUniqueKey && !KeyExists(theKey))
            {
                SortKeyNumList.Add(new SortKeyNum
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

        public void InsertIfCountMatchesMax()
        {
            if (SortKeyNumList.Count == MaxBatchSize)
            {
                InsertBulk();
            }
        }

        public void InsertAnyLeftOvers()
        {
            if (SortKeyNumList.Count > 0)
            {
                InsertBulk();
            }
        }

        public void InsertBulk()
        {
            SortKeyNumCollection.InsertBulk(SortKeyNumList);
            SortKeyNumList.Clear();
        }

        public bool KeyExists(long theKey)
        {
            bool kExists = SortKeyNumCollection.FindOne(x => x.Key == theKey) != null;
            if (!kExists)
            {
                kExists = SortKeyNumList.Exists(x => x.Key == theKey);
            }
            return kExists;
        }
    }


    public class SortKeyBulkInserter<T> : IDisposable
    {
        const int MaxBatchSize = 5000;
        bool disposed;
        bool hasUniqueKey;
        public List<SortKey<T>> SortKeyList { get; set; }

        public LiteDatabase SortDb { get; set; }

        public LiteCollection<SortKey<T>> SortKeyCollection { get; set; }

        public SortKeyBulkInserter(string connStr, string collectionName = Constants.SortCollectionName, bool uniqueKey = false)
        {
            SortKeyList = new List<SortKey<T>>();
            SortDb = new LiteDatabase(connStr);
            SortKeyCollection = SortDb.GetCollection<SortKey<T>>(collectionName);
            SortKeyCollection.EnsureIndex(x => x.Key, uniqueKey);
            hasUniqueKey = uniqueKey;

        }

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                SortDb.Dispose();
            }

            disposed = true;
        }

        public string Add(T theKey, string theData)
        {
            string dupeLine = string.Empty;
            if (!hasUniqueKey || hasUniqueKey && !KeyExists(theKey))
            {
                SortKeyList.Add(new SortKey<T>
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

        public void InsertIfCountMatchesMax()
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

        public void InsertBulk()
        {
            SortKeyCollection.InsertBulk(SortKeyList);
            SortKeyList.Clear();
        }

        public bool KeyExists(T theKey)
        {
            bool kExists = SortKeyCollection.FindOne(x => x.Key.Equals(theKey)) != null;
            if (!kExists)
            {
                kExists = SortKeyList.Exists(x => x.Key.Equals(theKey));
            }
            return kExists;
        }
    }



}