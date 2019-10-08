using System;
using System.Collections.Generic;
using System.Text;
using Murmur;

namespace NBloom.Hashing
{
    public class MurmurHash<TInput> : IHashFunction<TInput>
    {
        private static uint _hashSeed = 0;
        private readonly Murmur32 _hashFunction = MurmurHash.Create32(_hashSeed++);
        private readonly Func<TInput, byte[]> _inputToBytes;

        public MurmurHash(Func<TInput, byte[]> inputToBytes)
        {
            _inputToBytes = inputToBytes;
        }

        public uint GenerateHash(TInput value)
        {
            var bytes = _inputToBytes(value);
            var hashBytes = _hashFunction.ComputeHash(bytes);
            var hashUint = BitConverter.ToUInt32(hashBytes, 0);

            return hashUint;
        }
    }
}
