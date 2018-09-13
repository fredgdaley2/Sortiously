namespace Sortiously.Framework
{
    public class DelimitedLineParserResult
    {
        public string[] Values { get; set; }

        public bool Errored { get; set; }

        public string ErrorMessage { get; set; }
    }
}
