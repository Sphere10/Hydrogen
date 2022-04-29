using System;

namespace Sphere10.Framework {

	public abstract class DigitalSignatureSchemeDecorator<TPrivateKey, TPublicKey, TScheme> : IDigitalSignatureScheme
    where TPrivateKey : IPrivateKey
    where TPublicKey : IPublicKey
	where TScheme : IDigitalSignatureScheme {

        protected readonly IDigitalSignatureScheme Internal;

        protected DigitalSignatureSchemeDecorator(TScheme internalScheme) {
            Internal = internalScheme;
        }

        public virtual DigitalSignatureSchemeTraits Traits => Internal.Traits;

		public virtual IIESAlgorithm IES => Internal.IES;

		public virtual IPrivateKey CreatePrivateKey(ReadOnlySpan<byte> secret256) {
            return Internal.CreatePrivateKey(secret256);
        }

        public virtual IPublicKey DerivePublicKey(IPrivateKey privateKey, ulong signerNonce) {
            return Internal.DerivePublicKey(privateKey, signerNonce);
        }

        public virtual bool IsPublicKey(IPrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes) {
            return Internal.IsPublicKey(privateKey, publicKeyBytes);
        }

        public virtual bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out IPrivateKey privateKey) {
            return Internal.TryParsePrivateKey(bytes, out privateKey);
        }

        public virtual bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out IPublicKey publicKey) {
            return Internal.TryParsePublicKey(bytes, out publicKey);
        }

        public virtual byte[] SignDigest(IPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ulong signerNonce) {
            return Internal.SignDigest(privateKey, messageDigest,  signerNonce);
        }

        public virtual byte[] CalculateMessageDigest(ReadOnlySpan<byte> message)
            => Internal.CalculateMessageDigest(message);

        public virtual bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey) {
            return Internal.VerifyDigest(signature, messageDigest, publicKey);
        }
    }


}
