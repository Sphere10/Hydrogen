using System;

namespace Hydrogen {

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

		protected StatelessDigitalSignatureScheme(CHF messageDigestCHF) : base(messageDigestCHF) {
            Traits = Traits & DigitalSignatureSchemeTraits.Stateless;
        }

        public sealed override TPublicKey DerivePublicKey(TPrivateKey privateKey, ulong signerNonce)
            => DerivePublicKey(privateKey);

        public abstract TPublicKey DerivePublicKey(TPrivateKey privateKey);

        public sealed override byte[] SignDigest(TPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ulong signerNonce)
            => SignDigest(privateKey, messageDigest);

        public abstract byte[] SignDigest(TPrivateKey privateKey, ReadOnlySpan<byte> messageDigest);
        
    }

}
