using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBloom.Hashing
{
    internal class MurmurHashFactory<T> : IHashFunctionFactory<T>
    {
        private readonly Func<T, byte[]> _inputToBytes;

        public MurmurHashFactory(Func<T, byte[]> inputToBytes)
        {
            _inputToBytes = inputToBytes;
        }

        public IHashFunction<T>[] GenerateHashFunctions(ushort quantity)
        {
            return Enumerable.Range(0, quantity).Select(x => new MurmurHash<T>(_inputToBytes)).ToArray();
        }
    }
}
