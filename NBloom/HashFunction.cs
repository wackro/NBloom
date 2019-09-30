using System;

namespace NBloom
{
    public struct HashFunction<TInput>
    {
        internal Func<TInput, uint> GenerateHash { get; }

        public HashFunction(Func<TInput, uint> generateHash)
        {
            if(generateHash == null)
            {
                throw new ArgumentNullException(nameof(generateHash));
            }

            GenerateHash = generateHash;
        }
    }
}
