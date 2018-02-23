using System;

namespace Sortiously
{

    public class DelimitedFileSource<T> : IFileSource, IDelimitedKey<T>
    {
        public string SourceFilePath { get; set; }

        public string Delimiter { get; set; }

        public bool HasHeader { get; set; }

        public Func<string[], string, T> GetKey { get; set; }


    }

}
