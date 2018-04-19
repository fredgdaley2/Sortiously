using Sortiously.Interfaces;

namespace Sortiously
{
    public class MasterFixedWidthFileSource<T> : FixedWidthFileSource<T>, ISortableFile
    {
        public SortDirection SortDirection { get; set; }
        public int MaxBatchSize { get; set; } = 250000;
    }

}