using System;
using System.Collections.Generic;

namespace NBloom.Hashing
{
    public interface IHashFunctionFactory<T>
    {
        IHashFunction<T>[] GenerateHashFunctions(ushort quantity);
    }
}
