using System;

namespace NBloom.Hashing
{
    public interface IHashFunction<TInput>
    {
        uint GenerateHash(TInput value);
    }
}
