using System;

namespace Sphere10.Framework {

    /// <summary>
    /// Represents a stateless digital signature scheme. One where knowledge of nonces from prior signatures are not ever needed. These
    /// are mainly ECDSA, Schnorr and SPHINCS+.
    /// </summary>
    /// <typeparam name="TPrivateKey"></typeparam>
    /// <typeparam name="TPublicKey"></typeparam>
    public abstract class StatelessDigitalSignatureScheme<TPrivateKey, TPublicKey> : DigitalSignatureSchemeBase<TPrivateKey, TPublicKey>
        where TPrivateKey : IPrivateKey
        where TPublicKey : IPublicKey {

        protected const int DefaultNonce = 0;

        public StatelessDigitalSignatureScheme(CHF messageDigestCHF, int saltSizeBytes = 0) 
            : base(messageDigestCHF, saltSizeBytes) {
        }

        public sealed override TPublicKey DerivePublicKey(TPrivateKey privateKey, ulong signerNonce)
            => DerivePublicKey(privateKey);

        public abstract TPublicKey DerivePublicKey(TPrivateKey privateKey);

        public sealed override byte[] SignDigest(TPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> salt, ulong signerNonce)
            => SignDigest(privateKey, messageDigest, salt);

        public abstract byte[] SignDigest(TPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> salt);
        
    }

}
