using System;
using System.Linq;
using Xunit;

namespace NBloom.Test
{
    public class SimpleBloomFilterTests
    {
        private static uint c = 0;
        private readonly Func<string, uint> mockHashFunctionDelegate = x => c++;

        [Theory]
        [InlineData(1)]
        [InlineData(145345)]
        [InlineData(999999)]
        public void Initialisation__BitVecorSizeGreaterThanOne__InitialisesBitVectorToThatValue(int bitVectorSize)
        {
            var bloomFilter = new SimpleBloomFilter((uint)bitVectorSize, GenerateMockHashFunctions(bitVectorSize));

            Assert.Equal(bitVectorSize, bloomFilter.BitVector.Length);
        }

        [Fact]
        public void Initialisation__BitVecorSizeOfOne__ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SimpleBloomFilter(0));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(145345)]
        [InlineData(999999)]
        public void Initialisation__BitVectorAnySize__ContainsAllZeroes(int bitVectorSize)
        {
            var expectedInitialValue = false;

            var bloomFilter = new SimpleBloomFilter((uint)bitVectorSize, GenerateMockHashFunctions(bitVectorSize));

            Assert.All(bloomFilter.BitVector, x => Assert.Equal(expectedInitialValue, x));
        }

        [Fact]
        public void Initialisation__NullHashFunctions__ThrowsArgumentNullException()
        {
            var hashFunctions = (HashFunction[])null;

            Assert.Throws<ArgumentNullException>(() => new SimpleBloomFilter(5, hashFunctions));
        }

        [Fact]
        public void Initialisation__InitialisedHashFunctionsWithSomeNulls__ThrowsArgumentNullException()
        {
            var hashFunctions = new HashFunction[]
            {
                new HashFunction(mockHashFunctionDelegate),
                new HashFunction(mockHashFunctionDelegate),
                null
            };

            Assert.Throws<ArgumentException>(() => new SimpleBloomFilter(5, hashFunctions));
        }

        [Fact]
        public void Initialisation__NumberOfHashFunctionsMoreThanBitVectorSize__ThrowsArgumentException()
        {
            var hashFunctions = GenerateMockHashFunctions(3);

            Assert.Throws<ArgumentException>(() => new SimpleBloomFilter(2, hashFunctions));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(81973489u, 50)]
        [InlineData(776911693u, 9999)]
        public void ConvertToIndex__AnyValue__MapsToIntegerWithinRangeOfBitVector(uint hash, uint bitVectorSize)
        {
            var bloomFilter = new SimpleBloomFilter(bitVectorSize, GenerateMockHashFunctions(1));

            var index = bloomFilter.ConvertToIndex(hash);

            Assert.InRange(index, 0u, (uint) bloomFilter.BitVector.Length - 1);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(71234616)]
        public void ConvertToIndex__IdenticalInputs__ReturnTheSameValue(uint hash)
        {
            var bloomFilter = new SimpleBloomFilter(5, GenerateMockHashFunctions(2));

            var expectedIndex = bloomFilter.ConvertToIndex(hash);

            var indexes = Enumerable.Range(0, 10).Select(x => bloomFilter.ConvertToIndex(hash));

            Assert.All(indexes, x => Assert.Equal(expectedIndex, x));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(9999)]
        public void Add__AnyValue__BitVectorHasBitsModified(int numHashFunctions)
        {
            var bloomFilter = new SimpleBloomFilter(999999, GenerateMockHashFunctions(numHashFunctions));

            bloomFilter.Add("097a6sdf0");

            var bitVectorModified = bloomFilter.BitVector.Any(x => x == true);

            Assert.True(bitVectorModified);
        }

        [Fact(Skip = "Need to test this from the inside by setting manipulating the bit vector and hash values")]
        public void Test__AfterAdding__NonAddedValueTestsAsFalse()
        {
            var inputs = new string[] { "cat", "dog", "horse", "pig", "chicken" };

            var bloomFilter = new SimpleBloomFilter(20, GenerateMockHashFunctions(2));

            foreach(var i in inputs)
            {
                bloomFilter.Add(i);
            }

            Assert.False(bloomFilter.Contains("donkey"));
        }

        [Fact]
        public void Clear__DirtyVector__ResultsInClearedVector()
        {
            var bloomfilter = new SimpleBloomFilter(20, GenerateMockHashFunctions(20));

            bloomfilter.BitVector[0] = false;
            bloomfilter.BitVector[5] = false;
            bloomfilter.BitVector[10] = false;
            bloomfilter.BitVector[15] = false;

            bloomfilter.Clear();

            Assert.All(bloomfilter.BitVector, x => Assert.False(x));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public void CalculateOptimalBitVectorSize__AnySetSize__ReturnsGreaterThanSetSize(uint setSize)
        {
            var optimal = SimpleBloomFilter.CalculateOptimalBitVectorSize(setSize, 0.5f);

            Assert.True(optimal > setSize);
        }

        private HashFunction[] GenerateMockHashFunctions(int number)
        {
            var hashFunctions = new HashFunction[number];

            for(var i = 0; i < number; i++)
            {
                hashFunctions[i] = new HashFunction(mockHashFunctionDelegate);
            }

            return hashFunctions;
        }
    }
}
