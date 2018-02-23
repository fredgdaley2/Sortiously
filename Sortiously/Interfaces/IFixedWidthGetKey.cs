using System;

namespace Sortiously
{
    public interface IFixedWidthGetKey<T>
    {
        Func<string, T> GetKey { get; set; }
    }

}