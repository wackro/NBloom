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

        public uint GenerateHash(TInput value)
        {
            var bytes = Encoding.ASCII.GetBytes(value.ToString());
            var hashBytes = _hashFunction.ComputeHash(bytes);
            var hashUint = BitConverter.ToUInt32(hashBytes, 0);

            return hashUint;
        }
    }
}
