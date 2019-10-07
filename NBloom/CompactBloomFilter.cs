using System;
using System.Collections;
using System.Linq;

namespace NBloom
{
    public class CompactBloomFilter<T> : BloomFilter<T>
    {
        internal readonly BitArray Vector;

        public CompactBloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> getBytesDelegate)
            : base(setSize, falsePositiveRate, getBytesDelegate)
        {
            Vector = new BitArray((int)VectorSize);
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
