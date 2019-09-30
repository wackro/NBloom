using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

namespace NBloom.Test
{
    public class CountingBloomFilterTests
    {
        [Fact]
        public void Add__AnyInput__IncrementsArrayPositionsByOne()
        {
            var mockHashes = new HashFunction<string>[]
            {
                new HashFunction<string>(x => 0),
                new HashFunction<string>(x => 1),
                new HashFunction<string>(x => 2)
            };

            var bloomFilter = new CountingBloomFilter<string>(10, mockHashes);

            bloomFilter.Add("test");

            Assert.Equal(1u, bloomFilter.Vector[0]);
            Assert.Equal(1u, bloomFilter.Vector[1]);
            Assert.Equal(1u, bloomFilter.Vector[2]);
        }

        [Fact]
        public void Remove__AnyInput__DecrementsArrayPositionsByOne()
        {
            var mockHashes = new HashFunction<string>[]
            {
                new HashFunction<string>(x => 0),
                new HashFunction<string>(x => 1),
                new HashFunction<string>(x => 2)
            };

            var bloomFilter = new CountingBloomFilter<string>(10, mockHashes);

            bloomFilter.Vector[0] = 1;
            bloomFilter.Vector[1] = 1;
            bloomFilter.Vector[2] = 1;

            bloomFilter.Remove("test");

            Assert.Equal(0u, bloomFilter.Vector[0]);
            Assert.Equal(0u, bloomFilter.Vector[1]);
            Assert.Equal(0u, bloomFilter.Vector[2]);
        }
    }
}
