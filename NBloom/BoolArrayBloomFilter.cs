using System;
using System.Collections.Generic;
using System.Linq;

namespace NBloom
{
    public class BoolArrayBloomFilter<T> : BloomFilter<T>
    {
        internal readonly bool[] Vector;

        public BoolArrayBloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> convertToBytes)
            : base(setSize, falsePositiveRate, convertToBytes)
        {
            Vector = new bool[OptimalVectorSize];
        }

        public override void Add(T input)
        {
            var indices = Hash(input);

            foreach (var index in indices)
            {
                Vector[index] = true;
            }
        }

        public override bool Contains(T value)
        {
            var indices = Hash(value);

            return indices.All(i => Vector[i]);
        }

        public override void Reset()
        {
            Array.Clear(Vector, 0, Vector.Length);
        }
    }
}
