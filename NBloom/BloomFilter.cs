using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBloom.Hashing;

namespace NBloom
{
    public abstract class BloomFilter<T>
    {
        public float ActualFalsePositiveRate => (float)Math.Pow(1 - Math.Exp(-(_hashFunctions.Length) * _setSize / VectorSize), _hashFunctions.Length);

        protected uint VectorSize { get; private set; }

        protected int HashCount => _hashFunctions.Length;

        private float _falsePositiveRate;
        private uint _setSize;
        private IHashFunction<T>[] _hashFunctions;

        protected BloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> inputToBytes)
        {
            if (inputToBytes == null)
            {
                throw new ArgumentNullException(nameof(inputToBytes));
            }

            Init(setSize, falsePositiveRate);
            InitDefaultHashFunctions(inputToBytes);
        }

        protected BloomFilter(uint setSize, float falsePositiveRate, IHashFunctionFactory<T> hashFunctionFactory)
        {
            if (hashFunctionFactory == null)
            {
                throw new ArgumentNullException(nameof(hashFunctionFactory));
            }

            Init(setSize, falsePositiveRate);
            InitCustomHashFunctions(hashFunctionFactory);
        }

        private void Init(uint setSize, float falsePositiveRate)
        {
            if (setSize == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(setSize), "Must be greater than 0");
            }

            if (falsePositiveRate < 0 || falsePositiveRate > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(falsePositiveRate), "Must be between 0 and 1");
            }

            _setSize = setSize;
            _falsePositiveRate = falsePositiveRate;

            VectorSize = OptimalVectorSize();
        }

        public abstract void Add(T input);

        public void Add(IEnumerable<T> inputs)
        {
            Parallel.ForEach(inputs, (i) => Add(i));
        }

        public abstract bool Contains(T input);

        protected IEnumerable<uint> Hash(T input) => _hashFunctions.Select(x => ToIndex(x.GenerateHash(input)));

        internal uint ToIndex(uint hash) => hash % VectorSize;

        internal uint OptimalVectorSize() => (uint)Math.Ceiling(_setSize * Math.Log(_falsePositiveRate) / Math.Log(1 / Math.Pow(2, Math.Log(2))));

        private uint OptimalHashCount() => OptimalVectorSize() / _setSize * (uint)Math.Log(2);

        private void InitDefaultHashFunctions(Func<T, byte[]> inputToBytes)
        {
            var hashCount = OptimalHashCount();
            _hashFunctions = new IHashFunction<T>[hashCount];

            for (var i = 0; i < hashCount; i++)
            {
                _hashFunctions[i] = new MurmurHash<T>(inputToBytes);
            }
        }

        private void InitCustomHashFunctions(IHashFunctionFactory<T> hashFunctionFactory)
        {
            _hashFunctions = hashFunctionFactory.GenerateHashFunctions(OptimalHashCount()).ToArray();
        }
    }
}