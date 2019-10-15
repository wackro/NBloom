using System;
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
            var bloomFilter = new CountingBloomFilter<string>(1000, 0.001f, x => Encoding.ASCII.GetBytes(x));
            var input = "test";

            var hashes = bloomFilter.Hash(input);

            bloomFilter.Add(input);

            Assert.All(hashes, (h) => Assert.Equal(1, bloomFilter.Vector[h]));
        }

        [Fact]
        public void Add__AddOnTopOfFullIndex__ShouldNotOverflow()
        {
            var bloomFilter = new CountingBloomFilter<string>(1000, 0.001f, x => Encoding.ASCII.GetBytes(x));

            bloomFilter.Vector[0] = byte.MaxValue;
            bloomFilter.Vector[1] = byte.MaxValue;
            bloomFilter.Vector[2] = byte.MaxValue;

            bloomFilter.Add("test");

            Assert.Equal(byte.MaxValue, bloomFilter.Vector[0]);
            Assert.Equal(byte.MaxValue, bloomFilter.Vector[1]);
            Assert.Equal(byte.MaxValue, bloomFilter.Vector[2]);
        }

        [Fact]
        public void Remove__AnyInput__DecrementsArrayPositionsByOne()
        {
            var bloomFilter = new CountingBloomFilter<string>(1000, 0.001f, x => Encoding.ASCII.GetBytes(x));
            
            for(var i = 0; i< bloomFilter.Vector.Length; i++)
            {
                bloomFilter.Vector[i] = 1;
            }

            var input = "test";

            var hashes = bloomFilter.Hash(input);

            bloomFilter.Remove(input);

            Assert.All(hashes, (h) => Assert.Equal(0, bloomFilter.Vector[h]));
        }

        [Fact]
        public void Remove__SomethingThatHasNotBeenAdded__DoesNotThrow()
        {
            var bloomFilter = new CountingBloomFilter<string>(1000, 0.001f, x => Encoding.ASCII.GetBytes(x));

            bloomFilter.Remove("test");
        }

        [Fact]
        public void Add__RemoveWhenIndicesAreZero__DoesNotUnderflow()
        {
            var bloomFilter = new CountingBloomFilter<string>(1000, 0.001f, x => Encoding.ASCII.GetBytes(x));

            Assert.Equal(0, bloomFilter.Vector[0]);
            Assert.Equal(0, bloomFilter.Vector[1]);
            Assert.Equal(0, bloomFilter.Vector[2]);

            bloomFilter.Remove("test");

            Assert.Equal(0, bloomFilter.Vector[0]);
            Assert.Equal(0, bloomFilter.Vector[1]);
            Assert.Equal(0, bloomFilter.Vector[2]);
        }
    }
}
