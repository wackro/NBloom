﻿using System;
using System.Linq;
using System.Text;

namespace NBloom
{
    public class CountingBloomFilter<T> : BloomFilter<T>
    {
        internal byte[] Vector;

        public CountingBloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> inputToBytes)
            : base(setSize, falsePositiveRate, inputToBytes)
        {
            Vector = new byte[OptimalVectorSize];
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
