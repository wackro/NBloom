using System;
using System.Linq;
using System.Threading.Tasks;

namespace NBloom
{
    public class SimpleBloomFilter<T> : BloomFilter<T>
    {
        internal override uint VectorSize => (uint)Vector.Length;

        internal readonly bool[] Vector;

        public SimpleBloomFilter(uint vectorSize, params HashFunction<T>[] hashFunctions)
            : base(vectorSize, hashFunctions)
        {
            Vector = new bool[vectorSize];
        }

        public SimpleBloomFilter(uint setSize, float falsePositiveRate, params HashFunction<T>[] hashFunctions)
            : base(setSize, falsePositiveRate, hashFunctions)
        {
            Vector = new bool[CalculateOptimalVectorSize(setSize, falsePositiveRate)];
        }

        public SimpleBloomFilter(uint setSize, uint vectorSize, params HashFunction<T>[] hashes)
            : base(setSize, vectorSize, hashes)
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
