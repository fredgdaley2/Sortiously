using System;

namespace Sortiously.Interfaces
{
    public interface IDelimitedKey<T>
    {
        Func<string[], string, T> GetKey { get; set; }

    }


}