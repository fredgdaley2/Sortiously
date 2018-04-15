using System.Collections.Generic;

namespace Sortiously
{
    public class SortDefinitions
    {
        private List<SortDefinition> Keys { get; set; }

        public List<SortDefinition> GetKeys()
        {
            return Keys;
        }

        public void Add(SortDefinition sortDef)
        {
            if (this.Keys == null)
            {
                Keys = new List<SortDefinition>();
                sortDef.IsLookUp = true;
            }

            Keys.Add(sortDef);
        }

        public void AddRange(IEnumerable<SortDefinition> sortDefinitions)
        {
            if (this.Keys == null)
            {
                Keys = new List<SortDefinition>();
                Keys.AddRange(sortDefinitions);
                Keys[0].IsLookUp = true;
            }
            else
            {
                Keys.AddRange(sortDefinitions);
            }
        }

        internal string BuildTableDefinitions()
        {
            List<string> tableDefs = new List<string>();
            for (int idx = 0; idx < Keys.Count; idx++)
            {
                if (Keys[idx].DataType == KeyType.AlphaNumeric)
                {
                    tableDefs.Add(string.Format("SortKey{0} TEXT NOT NULL", idx));
                }
                else
                {
                    tableDefs.Add(string.Format("SortKey{0} Integer NOT NULL", idx));
                }
            }

            return string.Join(",", tableDefs);
        }

        internal string BuildIndexDefinitions()
        {
            List<string> indexDefs = new List<string>();
            for (int idx = 0; idx < Keys.Count; idx++)
            {
                indexDefs.Add(string.Format("CREATE INDEX sortKey{0}_idx ON FileData (SortKey{0} {1})", idx, Keys[idx].Direction == SortDirection.Ascending ? "ASC" : "DESC"));
            }

            return string.Join(";", indexDefs);
        }

        internal string BuildInsertColumns()
        {
            List<string> insertCols = new List<string>();
            for (int idx = 0; idx < Keys.Count; idx++)
            {
                insertCols.Add(string.Format("SortKey{0}", idx));
            }

            return string.Join(",", insertCols);
        }

        internal string BuildInsertParamters()
        {
            List<string> insertParams = new List<string>();
            for (int idx = 0; idx < Keys.Count; idx++)
            {
                insertParams.Add(string.Format("@key{0}", idx));
            }

            return string.Join(",", insertParams);
        }

        internal string BuildOrderClause()
        {
            List<string> orderDefs = new List<string>();
            for (int idx = 0; idx < Keys.Count; idx++)
            {
                orderDefs.Add(string.Format("SortKey{0} {1}", idx, Keys[idx].Direction == SortDirection.Ascending ? "ASC" : "DESC"));
            }

            return string.Join(",", orderDefs);
        }
        //@"CREATE TABLE FileData ( Id Integer primary key autoincrement, SortKey TEXT NOT NULL, LineData TEXT NOT NULL);";
        //return string.Format(@"CREATE INDEX sortKey_idx ON FileData (SortKey {0});CREATE INDEX id_idx ON FileData (Id);", sortDirection == SortDirection.Ascending ? "ASC" : "DESC");
    }
}