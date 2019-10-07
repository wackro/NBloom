using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBloom.Hashing;

namespace NBloom
{
    public abstract class BloomFilter<T>
    {
        public double? FalsePositiveRate
        {
            get
            {
                if (_setSize.HasValue)
                {
                    _falsePositiveRate = Math.Pow(1 - Math.Exp(-(_hashFunctions.Length) * _setSize.Value / (double)VectorSize), _hashFunctions.Length);
                }
                
                return _falsePositiveRate;
            }
        }

        protected abstract uint VectorSize { get; }

        private double? _falsePositiveRate;

        private readonly IHashFunction<T>[] _hashFunctions;

        private readonly uint? _setSize;

        internal BloomFilter(uint vectorSize, params IHashFunction<T>[] hashFunctions)
        {
            if (vectorSize == 0)
            {
                throw new ArgumentException("Must be greater than 0", nameof(vectorSize));
            }

            if (hashFunctions == null)
            {
                throw new ArgumentNullException(nameof(hashFunctions));
            }

            if (hashFunctions.Length > vectorSize)
            {
                throw new ArgumentException("Size of vector must be greater than the amount of hash functions", nameof(hashFunctions));
            }

            _hashFunctions = hashFunctions;
        }

        internal BloomFilter(uint setSize, float falsePositiveRate, params IHashFunction<T>[] hashFunctions)
            : this(CalculateOptimalVectorSize(setSize, falsePositiveRate), hashFunctions)
        {
            if (setSize == 0)
            {
                throw new ArgumentException(nameof(setSize));
            }

            if (falsePositiveRate < 0 || falsePositiveRate > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(falsePositiveRate), "Must be between 0 and 1");
            }

            _setSize = setSize;
        }

        internal BloomFilter(uint setSize, uint vectorSize, params IHashFunction<T>[] hashFunctions)
            : this(vectorSize, hashFunctions)
        {
            if (setSize == 0)
            {
                throw new ArgumentException(nameof(setSize));
            }

            _setSize = setSize;
        }

        public abstract void Add(T input);

        public void Add(IEnumerable<T> inputs)
        {
            Parallel.ForEach(inputs, (i) => Add(i));
        }

        public abstract bool Contains(T input);

        protected IEnumerable<uint> Hash(T input) => _hashFunctions.Select(x => ToIndex(x.GenerateHash(input)));

        internal static uint CalculateOptimalVectorSize(uint setSize, float falsePositiveRate)
        {
            var vectorSize = (uint)Math.Ceiling(setSize * Math.Log(falsePositiveRate) / Math.Log(1 / Math.Pow(2, Math.Log(2))));

            return vectorSize;
        }

        internal uint ToIndex(uint hash) => hash % VectorSize;
    }
}