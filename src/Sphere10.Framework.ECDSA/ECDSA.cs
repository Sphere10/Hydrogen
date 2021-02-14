using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sphere10.Framework.ECDSA
{

    public class ECDSA : StatelessDigitalSignatureScheme<ECDSA.PrivateKey, ECDSA.PublicKey>
    {
        private readonly ECDSAKeyType _keyType;

        public ECDSA(ECDSAKeyType keyType) : base(CHF.ConcatBytes) => _keyType = keyType;

        public override bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out PublicKey publicKey)
            => PublicKey.TryParse(bytes, out publicKey);

        public override bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out PrivateKey privateKey)
            => PrivateKey.TryParse(bytes, out privateKey);

        public override PrivateKey GeneratePrivateKey(ReadOnlySpan<byte> secret)
            => new(secret.ToArray());

        public override PublicKey DerivePublicKey(PrivateKey privateKey)
            => (PublicKey) ECDSAUtils.DoGetPublicKey(_keyType, privateKey);

        public override bool IsPublicKey(PrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes) =>
            ECDSAUtils.DoGetPublicKey(_keyType, privateKey).RawBytes.AsSpan().SequenceEqual(publicKeyBytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Sign(PrivateKey privateKey, ReadOnlySpan<byte> message)
        {
            var messageDigest = CalculateMessageDigest(message);
            return SignDigest(privateKey, messageDigest);
        }

        public override byte[] SignDigest(PrivateKey privateKey, ReadOnlySpan<byte> messageDigest)
            => ECDSAUtils.DoECDSASign(_keyType, privateKey, messageDigest);

        public override bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest,
            ReadOnlySpan<byte> publicKey)
            => ECDSAUtils.DoECDSAVerify(_keyType, signature, messageDigest, publicKey);

        public abstract class Key : IKey
        {
            protected Key(byte[] immutableRawBytes)
            {
                RawBytes = immutableRawBytes;
            }

            public readonly byte[] RawBytes;

            public override bool Equals(object obj)
            {
                if (obj is Key key)
                {
                    return Equals(key);
                }
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool Equals(Key other) => RawBytes.SequenceEqual(other.RawBytes);

            public override int GetHashCode() => (RawBytes != null ? RawBytes.GetHashCode() : 0);

            #region IKey

            byte[] IKey.RawBytes => RawBytes;

            #endregion
        }

        public class PrivateKey : Key, IPrivateKey
        {
            internal PrivateKey(byte[] rawKeyBytes) : base(rawKeyBytes)
            {
            }

            public static bool TryParse(ReadOnlySpan<byte> rawBytes, out PrivateKey privateKey)
            {
                privateKey = new PrivateKey(rawBytes.ToArray());
                return true;
            }
        }

        public class PublicKey : Key, IPublicKey
        {
            internal PublicKey(byte[] rawKeyBytes) : base(rawKeyBytes)
            {
            }

            public static bool TryParse(ReadOnlySpan<byte> rawBytes, out PublicKey publicKey)
            {
                publicKey = new PublicKey(rawBytes.ToArray());
                return true;
            }
        }
    }

}