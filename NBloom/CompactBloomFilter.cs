using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NBloom.Hashing;

namespace NBloom
{
    public class CompactBloomFilter<T> : BloomFilter<T>
    {
        protected override uint VectorSize => (uint)Vector.Length;

        internal readonly BitArray Vector;

        public CompactBloomFilter(uint vectorSize, params IHashFunction<T>[] hashFunctions)
            : base(vectorSize, hashFunctions)
        {
            Vector = new BitArray((int)vectorSize);
        }

        public CompactBloomFilter(uint setSize, float falsePositiveRate, params IHashFunction<T>[] hashFunctions)
            : base(setSize, falsePositiveRate, hashFunctions)
        {
            Vector = new BitArray((int)CalculateOptimalVectorSize(setSize, falsePositiveRate));
        }

        public CompactBloomFilter(uint setSize, uint vectorSize, params IHashFunction<T>[] hashFunctions)
            : base(setSize, vectorSize, hashFunctions)
        {
            Vector = new BitArray((int)vectorSize);
        }

        public override void Add(T input)
        {
            var indices = Hash(input);

            foreach (var index in indices)
            {
                Vector[(int)index] = true;
            }
        }

        public override bool Contains(T value)
        {
            var indices = Hash(value);

            return indices.All(i => Vector[(int)i]);
        }

        public void Clear()
        {
            Vector.SetAll(false);
        }
    }
}
