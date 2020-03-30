namespace NBloom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NBloom.Hashing;

    public abstract class BloomFilter<T>
    {
        private readonly float _falsePositiveRate;
        private readonly uint _setSize;
        private readonly IHashFunction<T>[] _hashFunctions;
        private readonly bool _threadsafe;

        private readonly object _addLock = new object();

        private uint? _optimalVectorSize;
        private ushort? _optimalHashCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="BloomFilter{T}"/> class.
        /// The default hashing function (Murmurhash) will be used.
        /// </summary>
        /// <param name="setSize">The number of elements in the bloom filter (m).</param>
        /// <param name="falsePositiveRate">The maximum false positive rate.</param>
        /// <param name="inputToBytes">A delegate to turn <typeparamref name="T"/> into a byte array. The delegate should return
        /// a reasonably unique byte array, so if <typeparamref name="T"/> is a complex type, make sure to use a unique property
        /// of that type.</param>
        /// <param name="threadSafe">If <see langword="true"/>, only one thread will be allowed to call Add() at a time.</param>
        protected BloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> inputToBytes, bool threadSafe = false)
            : this(setSize, falsePositiveRate, new MurmurHashFactory<T>(inputToBytes), threadSafe)
        {
            if (inputToBytes == null)
            {
                throw new ArgumentNullException(nameof(inputToBytes));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BloomFilter{T}"/> class.
        /// </summary>
        /// <param name="setSize">The number of elements in the bloom filter (m).</param>
        /// <param name="falsePositiveRate">The maximum false positive rate.</param>
        /// <param name="hashFunctionFactory">The custom hash function factory.</param>
        /// <param name="threadsafe">If <see langword="true"/>, only one thread will be allowed to call Add() at a time.</param>
        protected BloomFilter(uint setSize, float falsePositiveRate, IHashFunctionFactory<T> hashFunctionFactory, bool threadsafe = false)
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
            _threadsafe = threadsafe;
        }

        /// <summary>
        /// Gets the false positive rate for this bloom filter.
        /// </summary>
        /// <value>
        /// The false positive rate for this bloom filter.
        /// </value>
        public float FalsePositiveRate
        {
            get
            {
                return (float)Math.Pow(1 - Math.Exp(-_hashFunctions.Length / ((float)OptimalVectorSize / _setSize)), _hashFunctions.Length);
            }
        }

        /// <summary>
        /// Gets the  optimal number of hashes (k) for this bloom filter.
        /// </summary>
        /// <value>
        /// The  optimal number of hashes (k) for this bloom filter.
        /// </value>
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

        /// <summary>
        /// Gets the optimal vector size (m) for this bloom filter.
        /// </summary>
        /// <value>
        /// The optimal vector size (m) for this bloom filter.
        /// </value>
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

        /// <summary>
        /// Add an input into the bloom filter.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(T value)
        {
            if (!_threadsafe)
            {
                AddToVector(Hash(value));
            }
            else
            {
                lock (_addLock)
                {
                    AddToVector(Hash(value));
                }
            }
        }

        /// <summary>
        /// Add a set of inputs into the bloom filter.
        /// </summary>
        /// <param name="values">The set of values.</param>
        public void Add(IEnumerable<T> values)
        {
            if (!_threadsafe)
            {
                Parallel.ForEach(values, i => AddToVector(Hash(i)));
            }
            else
            {
                lock (_addLock)
                {
                    Parallel.ForEach(values, i => AddToVector(Hash(i)));
                }
            }
        }

        /// <summary>
        /// Check if the given input has been added to the bloom filter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><see langword="true"/> if the value is contained in the bloom filter.</returns>
        public bool Contains(T value) => VectorContains(Hash(value));

        /// <summary>
        /// Clear all values from the bloom filter.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Hash the given value into a set of integers between 0 and the vector size.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A set of integers representing vector indices.</returns>
        protected internal IEnumerable<uint> Hash(T value)
        {
            return _hashFunctions.Select(x => x.GenerateHash(value) % OptimalVectorSize).Distinct();
        }

        /// <summary>
        /// Add an input into the bloom filter.
        /// </summary>
        /// <param name="hash">The input.</param>
        protected abstract void AddToVector(IEnumerable<uint> hash);

        /// <summary>
        /// Check if the given hash has been added to the bloom filter.
        /// </summary>
        /// <param name="hash">The input.</param>
        /// <returns>True if it has been added. False if not.</returns>
        protected abstract bool VectorContains(IEnumerable<uint> hash);
    }
}