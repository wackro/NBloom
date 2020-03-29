using System;
using System.Linq;
using System.Text;
using Moq;
using Xunit;

namespace NBloom.Test
{
    public class BoolArrayBloomFilterTests
    {
        [Fact]
        public void Initialisation__JustInitialised__ContainsAllZeroes()
        {
            var bloomFilter = new BoolArrayBloomFilter<string>(10, 0.001f, x => Encoding.ASCII.GetBytes(x));

            Assert.DoesNotContain(true, bloomFilter.Vector);
        }

        [Fact]
        public void Initialisation__NullDelegate__ThrowsArgumentNullException()
        {
            var convertToBytesDelegate = (Func<string, byte[]>)null;

            Assert.Throws<ArgumentNullException>(() => new BoolArrayBloomFilter<string>(10000, 0.001f, convertToBytesDelegate));
        }

        [Theory]
        [InlineData(-0.1f)]
        [InlineData(1.1f)]
        public void Initialisation__FalsePositiveOutOfBounds__ThrowsArgumentOutOfRangeException(float falsePositiveRate)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BoolArrayBloomFilter<string>(10000, falsePositiveRate, x => Encoding.ASCII.GetBytes(x)));
        }

        [Fact]
        public void Initialisation__SetSizeOfZero__ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BoolArrayBloomFilter<string>(0, 0.001f, x => Encoding.ASCII.GetBytes(x)));
        }

        [Fact]
        public void Initialisation__HashCountAndVectorSize__AreOptimal()
        {
            var bloomFilter = new BoolArrayBloomFilter<string>(10000, 0.001f, x => Encoding.ASCII.GetBytes(x));

            Assert.Equal((uint)bloomFilter.OptimalHashCount, bloomFilter.OptimalHashCount);
            Assert.Equal((uint)bloomFilter.Vector.Length, bloomFilter.OptimalVectorSize);
        }

        [Theory(Skip = "is the formula for optimal m correct?")]
        [InlineData(0.1f)]
        [InlineData(0.01f)]
        [InlineData(0.001f)]
        [InlineData(0.0001f)]
        public void Initialisation__FalsePositiveRate__IsLessThanOrEqualToSpecified(float falsePositiveRate)
        {
            var bloomFilter = new BoolArrayBloomFilter<string>(10000, falsePositiveRate, x => Encoding.ASCII.GetBytes(x));

            Assert.True(bloomFilter.FalsePositiveRate <= falsePositiveRate);
        }

        [Fact]
        public void Reset__PopulatedVector__ResultsInClearedVector()
        {
            var bloomfilter = new BoolArrayBloomFilter<string>(10, 0.001f, x => Encoding.ASCII.GetBytes(x));

            bloomfilter.Vector[0] = true;
            bloomfilter.Vector[5] = true;
            bloomfilter.Vector[10] = true;
            bloomfilter.Vector[15] = true;

            bloomfilter.Reset();

            Assert.All(bloomfilter.Vector, x => Assert.False(x));
        }
    }
}
