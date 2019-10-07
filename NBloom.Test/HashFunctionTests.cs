using System;
using System.Linq;
using Xunit;
using NBloom.Hashing;

namespace NBloom.Test
{
    public class HashFunctionTests
    {
        [Fact]
        public void HashFunction__NullGenerateHashFunction__ThrowsArgumentNullException()
        {
            var func = (Func<string, uint>)null;

            Assert.Throws<ArgumentNullException>(() => new HashFunction<string>(func));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(76123944)]
        public void HashFunction__GenerateHash__ReturnsSameValueAsStoredFunc(uint hash)
        {
            var func = new Func<string, uint>(x => hash);
            var hashFunction = new HashFunction<string>(func);

            var resultFromFunc = func("test");
            var resultFromHashFunctionObject = hashFunction.GenerateHashDelegate("test");

            Assert.Equal(resultFromFunc, resultFromHashFunctionObject);
        }
    }
}
