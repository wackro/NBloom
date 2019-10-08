using System;
using System.Collections.Generic;

namespace NBloom.Hashing
{
    public interface IHashFunctionFactory<T>
    {
        IEnumerable<IHashFunction<T>> GenerateHashFunctions(uint quantity);
    }
}
