using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBloom.Hashing;

namespace NBloom
{
    public class BoolArrayBloomFilter<T> : BloomFilter<T>
    {
        protected override uint VectorSize => (uint)Vector.Length;

        internal readonly bool[] Vector;

        public BoolArrayBloomFilter(uint vectorSize, params IHashFunction<T>[] hashFunctions)
            : base(vectorSize, hashFunctions)
        {
            Vector = new bool[vectorSize];
        }

        public BoolArrayBloomFilter(uint setSize, float falsePositiveRate, params IHashFunction<T>[] hashFunctions)
            : base(setSize, falsePositiveRate, hashFunctions)
        {
            Vector = new bool[CalculateOptimalVectorSize(setSize, falsePositiveRate)];
        }

        public BoolArrayBloomFilter(uint setSize, uint vectorSize, params IHashFunction<T>[] hashFunctions)
            : base(setSize, vectorSize, hashFunctions)
        {
            Vector = new bool[vectorSize];
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

        public void Clear()
        {
            Array.Clear(Vector, 0, Vector.Length);
        }
    }
}
