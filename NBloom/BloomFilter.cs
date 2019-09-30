﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NBloom
{
    public abstract class BloomFilter<T>
    {
        public double? FalsePositiveProbability
        {
            get
            {
                var p = (double?)null;

                if (_setSize.HasValue)
                {
                    p = Math.Pow(1 - Math.Exp(-(HashFunctions.Length) * _setSize.Value / (double)VectorSize), HashFunctions.Length);
                }
                
                return p;
            }
        }

        internal abstract uint VectorSize { get; }

        internal HashFunction<T>[] HashFunctions { get; }

        private readonly uint? _setSize;

        public BloomFilter(uint vectorSize, params HashFunction<T>[] hashFunctions)
        {
            if (vectorSize == 0)
            {
                throw new ArgumentException("Must be greater than 0", nameof(vectorSize));
            }

            if (hashFunctions == null)
            {
                throw new ArgumentNullException(nameof(hashFunctions));
            }

            if (hashFunctions.Any(x => x == null))
            {
                throw new ArgumentException("All hash functions must be non-null");
            }

            if (hashFunctions.Length > vectorSize)
            {
                throw new ArgumentException("Size of vector must be greater than the amount of hash functions", nameof(hashFunctions));
            }

            HashFunctions = hashFunctions;
        }

        public BloomFilter(uint setSize, float falsePositiveRate, params HashFunction<T>[] hashFunctions)
            : this(CalculateOptimalVectorSize(setSize, falsePositiveRate), hashFunctions)
        {
            if (setSize == 0)
            {
                throw new ArgumentException(nameof(setSize));
            }

            if (falsePositiveRate == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(falsePositiveRate));
            }

            _setSize = setSize;
        }

        public BloomFilter(uint setSize, uint vectorSize, params HashFunction<T>[] hashes)
            : this(vectorSize, hashes)
        {
            if (setSize == 0)
            {
                throw new ArgumentException(nameof(setSize));
            }

            _setSize = setSize;
        }

        public abstract void Add(T input);

        public abstract bool Contains(T input);

        protected IEnumerable<uint> Hash(T input) => HashFunctions.Select(x => ToIndex(x.GenerateHash(input)));

        internal static uint CalculateOptimalVectorSize(uint setSize, float falsePositiveRate)
        {
            var vectorSize = (uint)Math.Ceiling(setSize * Math.Log(falsePositiveRate) / Math.Log(1 / Math.Pow(2, Math.Log(2))));

            return vectorSize;
        }

        internal uint ToIndex(uint hash) => hash % VectorSize;
    }
}