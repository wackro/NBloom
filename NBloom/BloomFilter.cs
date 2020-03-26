using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBloom.Hashing;

namespace NBloom
{
    public abstract class BloomFilter<T>
    {
        /// <summary>
        /// Returns the false positive rate for this bloom filter.
        /// </summary>
        public float FalsePositiveRate
        {
            get
            {
                return (float)Math.Pow(1 - Math.Exp(-_hashFunctions.Length / ((float)OptimalVectorSize / _setSize)), _hashFunctions.Length);
            }
        }

        private uint? _optimalVectorSize;

        /// <summary>
        /// Returns the optimal vector size (m) for this bloom filter.
        /// </summary>
        protected internal uint OptimalVectorSize
        {
            get
            {
                if (!_optimalVectorSize.HasValue)
                {
                    _optimalVectorSize = (uint)Math.Ceiling(_setSize * Math.Log(_falsePositiveRate) / Math.Log(1 / Math.Pow(2, Math.Log(2))));
                }

                return _optimalVectorSize.Value;
            }
        }

        private ushort? _optimalHashCount;

        /// <summary>
        /// Returns the optimal number of hashes (k) for this bloom filter.
        /// </summary>
        internal ushort OptimalHashCount
        {
            get
            {
                if (!_optimalHashCount.HasValue)
                {
                    _optimalHashCount = (ushort)(OptimalVectorSize / _setSize * Math.Log(2));
                }

                return _optimalHashCount.Value;
            }
        }

        private float _falsePositiveRate;
        private uint _setSize;
        private readonly IHashFunction<T>[] _hashFunctions;

        /// <summary>
        /// Instantiates a new bloom filter. The default hashing function (Murmurhash) will be used.
        /// </summary>
        /// <param name="setSize">The number of elements in the bloom filter (m)</param>
        /// <param name="falsePositiveRate">The maximum false positive rate</param>
        /// <param name="inputToBytes">A delegate to turn <typeparamref name="T"/> into a byte array.</param>
        protected BloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> inputToBytes)
            : this (setSize, falsePositiveRate, new MurmurHashFactory<T>(inputToBytes))
        {
            if (inputToBytes == null)
            {
                throw new ArgumentNullException(nameof(inputToBytes));
            }
        }

        /// <summary>
        /// Instantiates a new bloom fitler with a custom hashing factory.
        /// </summary>
        /// <param name="setSize">The number of elements in the bloom filter (m)</param>
        /// <param name="falsePositiveRate">The maximum false positive rate</param>
        /// <param name="hashFunctionFactory">The custom hash function factory</param>
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

        /// <summary>
        /// Add an input into the bloom filter.
        /// </summary>
        /// <param name="input">The input</param>
        public abstract void Add(T input);

        /// <summary>
        /// Add a set of inputs into the bloom filter.
        /// </summary>
        /// <param name="inputs">The set of inputs</param>
        public void Add(IEnumerable<T> inputs)
        {
            Parallel.ForEach(inputs, i => Add(i));
        }

        /// <summary>
        /// Check if the given input has been added to the bloom filter.
        /// </summary>
        /// <param name="input">The input</param>
        /// <returns>True if it has been added. False if not.</returns>
        public abstract bool Contains(T input);


        /// <summary>
        /// Reset the bloom filter to its original state.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Return the indices in the bloom filter that should be 'set' for the given input.
        /// </summary>
        /// <param name="input">The input</param>
        /// <returns>The hash</returns>
        protected internal IEnumerable<uint> Hash(T input)
        {
            return _hashFunctions.Select(x => x.GenerateHash(input) % OptimalVectorSize).Distinct();
        }
    }
}