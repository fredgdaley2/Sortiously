namespace Sortiously.Interfaces
{
    public interface ISortableFile
    {
        SortDirection SortDirection { get; set; }
        int MaxBatchSize { get; set; }
    }
}