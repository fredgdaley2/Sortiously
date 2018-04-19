namespace Sortiously
{
    public class SortDefinition
    {
        public KeyType DataType { get; set; }

        public SortDirection Direction { get; set; }

        public bool IsUniqueKey { get; set; }

        internal bool IsLookUp { get; set; }
    }
}