

namespace Sortiously
{
    internal class SortKey<T>
    {
        public long Id { get; set; }

        public T Key { get; set; }

        public string Data { get; set; }

        public bool Found { get; set; }
    }
}