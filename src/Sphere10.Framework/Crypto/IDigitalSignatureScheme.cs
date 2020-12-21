using System;
using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

	public interface IDigitalSignatureScheme {
		DigitalSignatureSchemeTraits Traits { get; }
		bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out IPublicKey publicKey);
		bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out IPrivateKey privateKey);
		IPrivateKey CreatePrivateKey(ReadOnlySpan<byte> secret256);
		IPublicKey DerivePublicKey(IPrivateKey privateKey, ulong signerNonce);
		bool IsPublicKey(IPrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes);
		byte[] GenerateSalt(ReadOnlySpan<byte> message);
		ReadOnlySpan<byte> ExtractSaltFromSignature(ReadOnlySpan<byte> signature);
		byte[] CalculateMessageDigest(ReadOnlySpan<byte> message, ReadOnlySpan<byte> salt);
		byte[] SignDigest(IPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> salt, ulong signerNonce);
		bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey);
	}

	public static class IDigitalSignatureSchemeExtensins {

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IPrivateKey GeneratePrivateKey(this IDigitalSignatureScheme dss)
			=> dss.CreatePrivateKey(Tools.Crypto.GenerateCryptographicallyRandomBytes(32));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Sign(this IDigitalSignatureScheme dss, IPrivateKey privateKey, ReadOnlySpan<byte> message, ulong signerNonce) {
			var salted = dss.Traits.HasFlag(DigitalSignatureSchemeTraits.SaltedSignatures);
			var salt = salted ? dss.GenerateSalt(message) : null;
			var messageDigest = dss.CalculateMessageDigest(message, salt);
			return dss.SignDigest(privateKey, messageDigest, salt, signerNonce);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Verify(this IDigitalSignatureScheme dss, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> message, IPublicKey publicKey)
			=> dss.Verify(signature, message, publicKey.RawBytes);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Verify(this IDigitalSignatureScheme dss, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> message, ReadOnlySpan<byte> publicKey) {
			var messageDigest = dss.CalculateMessageDigest(message, dss.Traits.HasFlag(DigitalSignatureSchemeTraits.SaltedSignatures) ? dss.ExtractSaltFromSignature(signature) : null);
			return dss.VerifyDigest(signature, messageDigest, publicKey);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool VerifyDigest(this IDigitalSignatureScheme dss, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, IPublicKey publicKey)
			=> dss.VerifyDigest(signature, messageDigest, publicKey.RawBytes);

	}
}
