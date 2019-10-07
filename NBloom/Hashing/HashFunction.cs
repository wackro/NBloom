using System;

namespace NBloom.Hashing
{
    public class HashFunction<TInput> : IHashFunction<TInput>
    {
        private Func<TInput, uint> _generateHashDelegate;

        public HashFunction(Func<TInput, uint> generateHashDelegate)
        {
            if (generateHashDelegate == null)
            {
                throw new ArgumentNullException(nameof(generateHashDelegate));
            }

            _generateHashDelegate = generateHashDelegate;
        }

        public uint GenerateHash(TInput value) => _generateHashDelegate(value);
    }
}
