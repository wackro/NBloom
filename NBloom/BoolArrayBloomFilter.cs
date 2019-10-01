using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBloom
{
    public class BoolArrayBloomFilter<T> : BloomFilter<T>
    {
        protected override uint VectorSize => (uint)Vector.Length;

        internal readonly bool[] Vector;

        public BoolArrayBloomFilter(uint vectorSize, params HashFunction<T>[] hashFunctions)
            : base(vectorSize, hashFunctions)
        {
            Vector = new bool[vectorSize];
        }

        public BoolArrayBloomFilter(uint setSize, float falsePositiveRate, params HashFunction<T>[] hashFunctions)
            : base(setSize, falsePositiveRate, hashFunctions)
        {
            Vector = new bool[CalculateOptimalVectorSize(setSize, falsePositiveRate)];
        }

        public BoolArrayBloomFilter(uint setSize, uint vectorSize, params HashFunction<T>[] hashes)
            : base(setSize, vectorSize, hashes)
        {
            Vector = new bool[vectorSize];
        }

        protected override void Add(IEnumerable<uint> indices)
        {
            foreach (var index in indices)
            {
                Vector[index] = true;
            }
        }

        protected override bool Contains(IEnumerable<uint> indices)
        {
            return indices.All(i => Vector[i]);
        }

        public override void Clear()
        {
            Array.Clear(Vector, 0, Vector.Length);
        }
    }
}
