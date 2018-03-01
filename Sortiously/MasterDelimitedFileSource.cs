using Sortiously.Interfaces;

namespace Sortiously
{
    public class MasterDelimitedFileSource<T> : DelimitedFileSource<T>, ISortableFile
    {
        public SortDirection SortDirection { get; set; }
    }
}