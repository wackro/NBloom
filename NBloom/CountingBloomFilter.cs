using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBloom
{
    public class CountingBloomFilter<T> : BloomFilter<T>
    {
        internal byte[] Vector;

        public CountingBloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> inputToBytes, bool threadsafe = false)
            : base(setSize, falsePositiveRate, inputToBytes, threadsafe)
        {
            Vector = new byte[OptimalVectorSize];
        }

        protected override void AddToVector(IEnumerable<uint> hash)
        {
            foreach(var i in hash)
            {
                if (Vector[i] != byte.MaxValue)
                {
                    Vector[i]++;
                }
            }
        }

        protected override bool VectorContains(IEnumerable<uint> hash)
        {
            return hash.All(i => Vector[i] > 0);
        }

        /// <summary>
        /// Remove a value fron the bloom filter.
        /// </summary>
        /// <param name="value">The value to remove</param>
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

        public override void Clear()
        {
            Array.Clear(Vector, 0, Vector.Length);
        }
    }
}
