namespace NBloom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BoolArrayBloomFilter<T> : BloomFilter<T>
    {
        internal readonly bool[] Vector;

        public BoolArrayBloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> convertToBytes, bool threadsafe = false)
            : base(setSize, falsePositiveRate, convertToBytes, threadsafe)
        {
            Vector = new bool[OptimalVectorSize];
        }

        public override void Clear()
        {
            Array.Clear(Vector, 0, Vector.Length);
        }

        protected override void AddToVector(IEnumerable<uint> hash)
        {
            foreach (var index in hash)
            {
                Vector[index] = true;
            }
        }

        protected override bool VectorContains(IEnumerable<uint> hash)
        {
            return hash.All(i => Vector[i]);
        }
    }
}
