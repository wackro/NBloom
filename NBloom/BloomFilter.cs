using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBloom.Hashing;

namespace NBloom
{
    public abstract class BloomFilter<T>
    {
        public float FalsePositiveRate
        {
            get
            {
                return (float)Math.Pow(1 - Math.Exp(-_hashFunctions.Length / ((float)OptimalVectorSize / _setSize)), _hashFunctions.Length);
            }
        }

        protected internal int HashCount => _hashFunctions.Length;

        protected internal uint OptimalVectorSize
        {
            get
            {
                return (uint)Math.Ceiling(_setSize * Math.Log(_falsePositiveRate) / Math.Log(1 / Math.Pow(2, Math.Log(2))));
            }
        }

        internal ushort OptimalHashCount
        {
            get
            {
                return (ushort)(OptimalVectorSize / _setSize * Math.Log(2));
            }
        }

        private float _falsePositiveRate;
        private uint _setSize;
        private readonly IHashFunction<T>[] _hashFunctions;

        protected BloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> inputToBytes)
            : this (setSize, falsePositiveRate, new MurmurHashFactory<T>(inputToBytes))
        {
            if (inputToBytes == null)
            {
                throw new ArgumentNullException(nameof(inputToBytes));
            }
        }

        protected BloomFilter(uint setSize, float falsePositiveRate, IHashFunctionFactory<T> hashFunctionFactory)
        {
            if (setSize == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(setSize), "Must be greater than 0");
            }

            if (falsePositiveRate < 0 || falsePositiveRate > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(falsePositiveRate), "Must be between 0 and 1");
            }

            if (hashFunctionFactory == null)
            {
                throw new ArgumentNullException(nameof(hashFunctionFactory));
            }

            _setSize = setSize;
            _falsePositiveRate = falsePositiveRate;
            _hashFunctions = hashFunctionFactory.GenerateHashFunctions(OptimalHashCount);
        }

        public abstract void Add(T input);

        public void Add(IEnumerable<T> inputs)
        {
            Parallel.ForEach(inputs, i => Add(i));
        }

        public abstract bool Contains(T input);

        protected IEnumerable<uint> Hash(T input)
        {
            return _hashFunctions.Select(x => x.GenerateHash(input) % (uint)_hashFunctions.Length);
        }
    }
}