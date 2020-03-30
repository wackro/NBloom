namespace NBloom
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class CompactBloomFilter<T> : BloomFilter<T>
    {
        internal readonly BitArray Vector;

        public CompactBloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> inputToBytes, bool threadsafe = false)
            : base(setSize, falsePositiveRate, inputToBytes, threadsafe)
        {
            Vector = new BitArray((int)OptimalVectorSize);
        }

        public override void Clear()
        {
            Vector.SetAll(false);
        }

        protected override void AddToVector(IEnumerable<uint> hash)
        {
            foreach (var index in hash)
            {
                Vector[(int)index] = true;
            }
        }

        protected override bool VectorContains(IEnumerable<uint> hash)
        {
            return hash.All(i => Vector[(int)i]);
        }
    }
}
