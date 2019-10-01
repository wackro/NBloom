using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBloom
{
    public class CountingBloomFilter<T> : BloomFilter<T>
    {
        protected override uint VectorSize => (uint)Vector.Length;

        internal ushort[] Vector { get; }

        public CountingBloomFilter(uint vectorSize, params HashFunction<T>[] hashFunctions)
            : base(vectorSize, hashFunctions)
        {
            Vector = new ushort[vectorSize];
        }

        public CountingBloomFilter(uint setSize, float falsePositiveRate, params HashFunction<T>[] hashFunctions)
            : base(setSize, falsePositiveRate, hashFunctions)
        {
            Vector = new ushort[CalculateOptimalVectorSize(setSize, falsePositiveRate)];
        }

        public CountingBloomFilter(uint setSize, uint vectorSize, params HashFunction<T>[] hashes)
            : base(setSize, vectorSize, hashes)
        {
            Vector = new ushort[vectorSize];
        }

        protected override void Add(IEnumerable<uint> indices)
        {
            foreach(var i in indices)
            {
                if (Vector[i] != ushort.MaxValue)
                {
                    Vector[i]++;
                }
            }
        }

        protected override bool Contains(IEnumerable<uint> indices)
        {
            return indices.All(i => Vector[i] > 0);
        }

        public void Remove(T input)
        {
            var indices = Hash(input);

            foreach(var i in indices)
            {
                if (Vector[i] != 0)
                {
                    Vector[i]--;
                }
            }
        }

        public override void Clear()
        {
            Array.Clear(Vector, 0, Vector.Length);
        }
    }
}
