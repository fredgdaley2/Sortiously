using LiteDB;

namespace DarthSortious
{
    public class SortKeyString
    {
        [BsonId]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }
    }
}

