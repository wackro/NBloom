using System;
using System.Linq;
using Xunit;

namespace BloomFilter.Test
{
    public class HashFunctionTests
    {
        [Fact]
        public void HashFunction__NullGenerateHashFunction__ThrowsArgumentNullException()
        {
            var func = (Func<string, string>)null;

            Assert.Throws<ArgumentNullException>(() => new HashFunction(func));
        }

        [Theory]
        [InlineData("a")]
        [InlineData("067asdftadg78t789afg")]
        public void HashFunction__GenerateHash__ReturnsSameValueAsStoredFunc(string hashValue)
        {
            var func = new Func<string, string>((x) => hashValue);
            var hashFunction = new HashFunction(func);

            var resultFromFunc = func("test");
            var resultFromHashFunctionObject = hashFunction.GenerateHash("test");

            Assert.Equal(resultFromFunc, resultFromHashFunctionObject);
        }
    }
}
