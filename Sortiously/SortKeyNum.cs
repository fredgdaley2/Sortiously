using LiteDB;

namespace DarthSortious
{
    public class SortKeyNum
    {
        [BsonId]
        public int Id { get; set; }
        public long Key { get; set; }
        public string Data { get; set; }
    }

}