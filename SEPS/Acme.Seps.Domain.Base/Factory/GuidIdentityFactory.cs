using Acme.Seps.Domain.Base.Entity;
using System;
using System.Security.Cryptography;

namespace Acme.Seps.Domain.Base.Factory
{
    /// <summary>
    /// <see href="http://www.codeproject.com/Articles/388157/GUIDs-as-fast-primary-keys-under-multiple-database">GUID Factory credits</see>
    /// </summary>
    public sealed class GuidIdentityFactory : IIdentityFactory<Guid>
    {
        private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();
        private readonly SequentialGuidType _sequentialGuidType;

        public GuidIdentityFactory(SequentialGuidType sequentialGuidType)
        {
            _sequentialGuidType = sequentialGuidType;
        }

        Guid IIdentityFactory<Guid>.CreateIdentity() => CreateGuid();

        private Guid CreateGuid()
        {
            var randomBytes = new byte[10];
            Rng.GetBytes(randomBytes);

            var timestamp = DateTime.UtcNow.Ticks / 10000L;
            var timestampBytes = BitConverter.GetBytes(timestamp);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(timestampBytes);

            var guidBytes = new byte[16];

            switch (_sequentialGuidType)
            {
                case SequentialGuidType.SequentialAsString:
                case SequentialGuidType.SequentialAsBinary:
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                    // If formatting as a string, we have to reverse the order
                    // of the Data1 and Data2 blocks on little-endian systems.
                    if (_sequentialGuidType == SequentialGuidType.SequentialAsString && BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(guidBytes, 0, 4);
                        Array.Reverse(guidBytes, 4, 2);
                    }
                    break;

                case SequentialGuidType.SequentialAtEnd:
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                    break;
            }

            var guidIdentity = new Guid(guidBytes);

            if (guidIdentity == default(Guid) ||
                guidIdentity.Equals(Guid.Empty))
                throw new DomainException("GUID identity must be initialized.");

            return guidIdentity;
        }
    }
}