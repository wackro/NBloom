using System;

namespace NBloom.Hashing
{
    public class HashFunction<TInput> : IHashFunction<TInput>
    {
        internal Func<TInput, uint> GenerateHashDelegate { get; }

        public HashFunction(Func<TInput, uint> generateHashDelegate)
        {
            if(generateHashDelegate == null)
            {
                throw new ArgumentNullException(nameof(generateHashDelegate));
            }

            GenerateHashDelegate = generateHashDelegate;
        }

        public uint GenerateHash(TInput value) => GenerateHashDelegate(value);
    }
}
