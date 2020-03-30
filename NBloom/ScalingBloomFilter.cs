namespace NBloom
{
    using System;
    using System.Collections.Generic;

    public class ScalingBloomFilter<T> : BloomFilter<T>
    {
        private const float FILL_RATIO = 0.5f;
        private const byte GROWTH_RATIO = 2;
        private const float TIGHTENING_RATIO = 0.8f;

        private bool[][] _vector;

        public ScalingBloomFilter(uint setSize, float falsePositiveRate, Func<T, byte[]> inputToBytes, bool threadsafe = false)
            : base(setSize, falsePositiveRate, inputToBytes, threadsafe)
        {
            InitVector();
            InitSlices();
        }

        public override void Clear() => InitVector();

        protected override void AddToVector(IEnumerable<uint> hash)
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

        protected override bool VectorContains(IEnumerable<uint> hash)
        {
            // if

            return true;
        }

        private static int RoundToNearestMultiple(uint num, int factor)
        {
            return (int)Math.Round(num / (double)factor, MidpointRounding.AwayFromZero) * factor;
        }

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
    }
}
