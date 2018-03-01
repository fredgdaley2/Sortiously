using Sortiously.Interfaces;

namespace Sortiously
{
    public class MasterFixedWidthFileSource<T> : FixedWidthFileSource<T>, ISortableFile
    {
        public SortDirection SortDirection { get; set; }
    }

}