﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NBloom
{
    public class ScalingBloomFilter<T> : BloomFilter<T>
    {
        private bool[][] _vector;

        private const float FILL_RATIO = 0.5f;
        private const byte GROWTH_RATIO = 2;
        private const float TIGHTENING_RATIO = 0.8f;

        public ScalingBloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> inputToBytes)
            : base(setSize, falsePositiveRate, inputToBytes)
        {
            InitVector();
            InitSlices();
        }

        public override void Add(T input)
        {
            // if(fill ratio isn't met)
            //  add it
            // }
            // else {
            //  if we don't have a next bloom filter instantiated {
            //      create one
            //  }
            //  add it to the next bloom filter
            //  }
        }

        public override bool Contains(T input)
        {
            // if

            return true;
        }

        public override void Reset() => InitVector();

        private void InitVector()
        {
            _vector = new bool[OptimalHashCount][];
        }

        private void InitSlices()
        {
            var sliceSize = RoundToNearestMultiple(OptimalVectorSize, OptimalHashCount);

            for (var i = 0; i < _vector.Length; i++)
            {
                _vector[i] = new bool[sliceSize];
            }
        }

        private static int RoundToNearestMultiple(uint num, int factor)
        {
            return (int)Math.Round(num / (double)factor, MidpointRounding.AwayFromZero) * factor;
        }
    }
}
