using System;

namespace NBloom
{
    public class HashFunction
    {
        internal Func<string, string> GenerateHash { get; }

        public HashFunction(Func<string, string> generateHash)
        {
            if(generateHash == null)
            {
                throw new ArgumentNullException(nameof(generateHash));
            }

            GenerateHash = generateHash;
        }
    }
}
