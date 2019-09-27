using System;
using System.Linq;

namespace NBloom
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

        public bool Contains(string value)
        {
            var indexes = HashFunctions.Select(x => ConvertToIndex(x.GenerateHash(value)));

            return indexes.All(i => BitVector[i]);
        }

        public void Clear()
        {
            for(var i = 0; i < BitVector.Length; i++)
            {
                BitVector[i] = false;
            }
        }

        internal int ConvertToIndex(string hash)
        {
            return Math.Abs(hash.GetHashCode()) % BitVector.Length;
        }
    }
}
