using System;

namespace Sortiously.Interfaces
{
    public interface IFixedWidthGetKey<T>
    {
        Func<string, T> GetKey { get; set; }
    }

}