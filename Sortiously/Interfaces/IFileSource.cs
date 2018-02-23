using System;

namespace Sortiously
{
    public interface IFileSource
    {
        string SourceFilePath { get; set; }

        bool HasHeader { get; set; }
    }
}