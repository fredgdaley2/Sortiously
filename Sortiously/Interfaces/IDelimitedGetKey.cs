using System;

namespace Sortiously
{
    public interface IDelimitedKey<T>
    {
        Func<string[], string, T> GetKey { get; set; }

    }


}