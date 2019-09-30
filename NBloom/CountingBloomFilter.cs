﻿using System;
using System.Linq;
using System.Text;

namespace NBloom
{
    public class CountingBloomFilter<T> : BloomFilter<T>
    {
        internal override uint VectorSize => (uint)Vector.Length;

        internal uint[] Vector { get; }

        public CountingBloomFilter(uint vectorSize, params HashFunction<T>[] hashFunctions)
            : base(vectorSize, hashFunctions)
        {
            Vector = new uint[vectorSize];
        }

        public CountingBloomFilter(uint setSize, float falsePositiveRate, params HashFunction<T>[] hashFunctions)
            : base(setSize, falsePositiveRate, hashFunctions)
        {
            Vector = new uint[CalculateOptimalVectorSize(setSize, falsePositiveRate)];
        }

        public CountingBloomFilter(uint setSize, uint vectorSize, params HashFunction<T>[] hashes)
            : base(setSize, vectorSize, hashes)
        {
            Vector = new uint[vectorSize];
        }

        public override void Add(T input)
        {
            var indices = Hash(input);

            foreach(var i in indices)
            {
                Vector[i]++;
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
                Vector[i]--;
            }
        }
    }
}