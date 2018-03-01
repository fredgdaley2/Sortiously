using System;

namespace Sortiously.Interfaces
{
    public interface IFileSource
    {
        string SourceFilePath { get; set; }

        bool HasHeader { get; set; }
    }
}