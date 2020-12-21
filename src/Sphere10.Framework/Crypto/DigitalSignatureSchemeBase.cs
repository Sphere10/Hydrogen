using System;
using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

	public abstract class DigitalSignatureSchemeBase<TPrivateKey, TPublicKey> : IDigitalSignatureScheme 
		where TPrivateKey : IPrivateKey 
		where TPublicKey : IPublicKey {
		private readonly CHF _mdCHF;
		private readonly int _saltSizeBytes;

		protected DigitalSignatureSchemeBase(CHF messageDigestCHF, int saltSizeBytes = 0) {
			_mdCHF = messageDigestCHF;
			_saltSizeBytes = saltSizeBytes;
			Traits = saltSizeBytes > 0 ? DigitalSignatureSchemeTraits.SaltedSignatures : 0;
		}

		public DigitalSignatureSchemeTraits Traits { get; }

		public abstract bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out TPublicKey publicKey);
		
		public abstract bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out TPrivateKey privateKey);
		
		public TPrivateKey GeneratePrivateKey()
			=> (TPrivateKey)((IDigitalSignatureScheme)this).GeneratePrivateKey();
	
		public abstract TPrivateKey GeneratePrivateKey(ReadOnlySpan<byte> seed);
		
		public abstract TPublicKey DerivePublicKey(TPrivateKey privateKey, ulong signerNonce);
		
		public abstract bool IsPublicKey(TPrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes);

		public virtual byte[] GenerateSalt(ReadOnlySpan<byte> message) {
			return Tools.Crypto.GenerateCryptographicallyRandomBytes(_saltSizeBytes);
		}

		public virtual ReadOnlySpan<byte> ExtractSaltFromSignature(ReadOnlySpan<byte> signature) {
			return signature.Slice(^_saltSizeBytes);
		}

		public virtual byte[] CalculateMessageDigest(ReadOnlySpan<byte> message, ReadOnlySpan<byte> salt)
			=> Traits == DigitalSignatureSchemeTraits.SaltedSignatures ? SMAC(message, salt) : Hashers.Hash(_mdCHF, message);

		public abstract byte[] SignDigest(TPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> salt, ulong signerNonce);

		public abstract bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey);

		bool IDigitalSignatureScheme.TryParsePublicKey(ReadOnlySpan<byte> bytes, out IPublicKey publicKey) {
			var result = TryParsePublicKey(bytes, out var pub);
			publicKey = pub;
			return result;
		}

		bool IDigitalSignatureScheme.TryParsePrivateKey(ReadOnlySpan<byte> bytes, out IPrivateKey privateKey) {
			var result = TryParsePrivateKey(bytes, out var priv);
			privateKey = priv;
			return result;
		}

		IPrivateKey IDigitalSignatureScheme.CreatePrivateKey(ReadOnlySpan<byte> seed)
			=> GeneratePrivateKey(seed);

		IPublicKey IDigitalSignatureScheme.DerivePublicKey(IPrivateKey privateKey, ulong signerNonce)
			=> DerivePublicKey((TPrivateKey)privateKey, signerNonce);

		bool IDigitalSignatureScheme.IsPublicKey(IPrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes)
			=> IsPublicKey((TPrivateKey)privateKey, publicKeyBytes);

		byte[] IDigitalSignatureScheme.SignDigest(IPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> salt, ulong signerNonce)
			=> SignDigest((TPrivateKey)privateKey, messageDigest, salt, signerNonce);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private byte[] SMAC(ReadOnlySpan<byte> message, ReadOnlySpan<byte> seed) {
			using (Hashers.BorrowHasher(_mdCHF, out var hasher)) {
				var digest = hasher.Compute(message);
				hasher.Transform(seed);
				hasher.Transform(digest);
				var innerHash = hasher.GetResult();
				hasher.Transform(seed);
				hasher.Transform(innerHash);
				return hasher.GetResult();
			}
		}

	}

}
