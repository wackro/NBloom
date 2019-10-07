//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace NBloom
//{
//    public class ScalingBloomFilter<T> : BloomFilter<T>
//    {
//        protected override uint VectorSize { get; }

//        private readonly bool[][] _vector;

//        private const float FILL_RATIO = 0.5f;
//        private const byte GROWTH_RATIO = 2;
//        private const float TIGHTENING_RATIO = 0.8f;

//        public ScalingBloomFilter(uint vectorSize, params HashFunction<T>[] hashFunctions)
//            : base(vectorSize, hashFunctions)
//        {
//            _vector = new bool[hashFunctions.Length][];

//            InitSlices(vectorSize, hashFunctions);
//        }

//        public ScalingBloomFilter(uint setSize, float falsePositiveRate, params HashFunction<T>[] hashFunctions)
//            : base(setSize, falsePositiveRate, hashFunctions)
//        {
//            _vector = new bool[hashFunctions.Length][];

//            InitSlices(CalculateOptimalVectorSize(setSize, falsePositiveRate), hashFunctions);
//        }

//        public ScalingBloomFilter(uint setSize, uint vectorSize, params HashFunction<T>[] hashFunctions)
//            : base(setSize, vectorSize, hashFunctions)
//        {
//            _vector = new bool[hashFunctions.Length][];

//            InitSlices(vectorSize, hashFunctions);
//        }

//        public override void Add(T input)
//        {
//            // if(fill ratio isn't met)
//            //  add it
//            // }
//            // else {
//            //  if we don't have a next bloom filter instantiated {
//            //      create one
//            //  }
//            //  add it to the next bloom filter
//            //  }
//        }

//        public override bool Contains(T input)
//        {
//            // if
//        }

//        private static int RoundToNearestMultiple(uint num, int factor)
//        {
//            return (int)Math.Round(num / (double)factor, MidpointRounding.AwayFromZero) * factor;
//        }

//        private void InitSlices(uint vectorSize, HashFunction<T>[] hashFunctions)
//        {
//            var sliceSize = RoundToNearestMultiple(vectorSize, hashFunctions.Length);
//            for (var i = 0; i < _vector.Length; i++)
//            {
//                _vector[i] = new bool[sliceSize];
//            }
//        }
//    }
//}
