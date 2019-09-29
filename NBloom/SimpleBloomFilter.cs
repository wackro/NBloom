using System;
using System.Linq;

namespace NBloom
{
    public class SimpleBloomFilter
    {
        internal bool[] BitVector { get; private set; }
        
        internal HashFunction[] HashFunctions { get; }

        internal double? FalsePositiveProbability
        {
            get
            {
                var p = (double?)null;

                if (_setSize.HasValue)
                {
                    p = Math.Pow(1 - Math.Exp(-(HashFunctions.Length) * _setSize.Value / (double)BitVector.Length), HashFunctions.Length);
                }

                return p;
            }
        }

        private readonly uint? _setSize;

        public SimpleBloomFilter(uint bitVectorSize, params HashFunction[] hashFunctions)
        {
            if(bitVectorSize == 0)
            {
                throw new ArgumentException("Must be greater than 0", nameof(bitVectorSize));
            }

            if (hashFunctions == null)
            {
                throw new ArgumentNullException(nameof(hashFunctions));
            }

            if(hashFunctions.Any(x => x == null))
            {
                throw new ArgumentException("All hash functions must be non-null");
            }

            if (hashFunctions.Length > bitVectorSize)
            {
                throw new ArgumentException("Size of bit vector must be greater than the amount of hash functions", nameof(hashFunctions));
            }

            BitVector = new bool[bitVectorSize];
            HashFunctions = hashFunctions;
        }

        public SimpleBloomFilter(uint setSize, float falsePositiveRate, params HashFunction[] hashFunctions)
            : this(CalculateOptimalBitVectorSize(setSize, falsePositiveRate), hashFunctions)
        {
            if (setSize == 0)
            {
                throw new ArgumentException(nameof(setSize));
            }

            if (falsePositiveRate == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(falsePositiveRate));
            }

            _setSize = setSize;
        }

        public SimpleBloomFilter(uint setSize, uint bitVectorSize, params HashFunction[] hashes)
            : this(bitVectorSize, hashes)
        {
            if (setSize == 0)
            {
                throw new ArgumentException(nameof(setSize));
            }

            _setSize = setSize;
        }

        public void Add(string input)
        {
            var indexes = HashFunctions.Select(x => ConvertToIndex(x.GenerateHash(input)));

            foreach (var index in indexes)
            {
                BitVector[index] = true;
            }
        }

        public bool Contains(string value)
        {
            var indexes = HashFunctions.Select(x => ConvertToIndex(x.GenerateHash(value)));

            return indexes.All(i => BitVector[i]);
        }

        public void Clear()
        {
            Enumerable.Range(0, BitVector.Length).Select((x, index) => BitVector[index] = false);
        }

        internal static uint CalculateOptimalBitVectorSize(uint setSize, float falseProbabilityRate)
        {
            var bitVectorSize = (uint)Math.Ceiling(setSize * Math.Log(falseProbabilityRate) / Math.Log(1 / Math.Pow(2, Math.Log(2))));

            return bitVectorSize;
        }

        internal uint ConvertToIndex(uint hash)
        {
            return hash % (uint) BitVector.Length;
        }
    }
}
