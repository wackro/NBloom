using System;
using System.Linq;

namespace BloomFilter
{
    public class SimpleBloomFilter
    {
        internal bool[] BitVector { get; }
        
        internal HashFunction[] HashFunctions { get; }

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

        public void Add(string input)
        {
            var indexes = HashFunctions.Select(x => ConvertToIndex(x.GenerateHash(input)));

            foreach (var index in indexes)
            {
                BitVector[index] = true;
            }
        }

        public bool Test(string value)
        {
            var indexes = HashFunctions.Select(x => ConvertToIndex(x.GenerateHash(value)));

            return indexes.All(i => BitVector[i]);
        }

        internal int ConvertToIndex(string hash)
        {
            return Math.Abs(hash.GetHashCode()) % BitVector.Length;
        }
    }
}
