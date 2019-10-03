using System;
using System.Linq;
using System.Text;

namespace NBloom
{
    public class CountingBloomFilter<T> : BloomFilter<T>
    {
        protected override uint VectorSize => (uint)Vector.Length;

        internal byte[] Vector { get; }

        public CountingBloomFilter(uint vectorSize, params HashFunction<T>[] hashFunctions)
            : base(vectorSize, hashFunctions)
        {
            Vector = new byte[vectorSize];
        }

        public CountingBloomFilter(uint setSize, float falsePositiveRate, params HashFunction<T>[] hashFunctions)
            : base(setSize, falsePositiveRate, hashFunctions)
        {
            Vector = new byte[CalculateOptimalVectorSize(setSize, falsePositiveRate)];
        }

        public CountingBloomFilter(uint setSize, uint vectorSize, params HashFunction<T>[] hashes)
            : base(setSize, vectorSize, hashes)
        {
            Vector = new byte[vectorSize];
        }

        public override void Add(T input)
        {
            var indices = Hash(input);

            foreach(var i in indices)
            {
                if (Vector[i] != byte.MaxValue)
                {
                    Vector[i]++;
                }
            }
        }

        public override bool Contains(T value)
        {
            var indices = Hash(value);

            return indices.All(i => Vector[i] > 0);
        }

        public void Remove(T value)
        {
            var indices = Hash(value);

            foreach(var i in indices)
            {
                if (Vector[i] != 0)
                {
                    Vector[i]--;
                }
            }
        }

        public void Clear()
        {
            Array.Clear(Vector, 0, Vector.Length);
        }
    }
}
