using System;

namespace NBloom
{
    public class HashFunction
    {
        internal Func<string, uint> GenerateHash { get; }

        public HashFunction(Func<string, uint> generateHash)
        {
            if(generateHash == null)
            {
                throw new ArgumentNullException(nameof(generateHash));
            }

            GenerateHash = generateHash;
        }
    }
}
