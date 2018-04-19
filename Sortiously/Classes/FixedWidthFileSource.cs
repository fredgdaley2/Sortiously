using System;
using Sortiously.Interfaces;

namespace Sortiously
{
    public class FixedWidthFileSource<T> : IFileSource, IFixedWidthGetKey<T>
    {
        public string SourceFilePath { get; set; }

        public int[] FixedWidths { get; set; }

        public bool HasHeader { get; set; }

        public Func<string, T> GetKey { get; set; }
    }

}