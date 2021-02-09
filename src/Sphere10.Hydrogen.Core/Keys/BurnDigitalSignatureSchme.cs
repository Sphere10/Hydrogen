using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Core.Keys {

	public sealed class BurnDigitalSignatureSchme : IDigitalSignatureScheme {
		public DigitalSignatureSchemeTraits Traits => DigitalSignatureSchemeTraits.None;


		public bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out IPublicKey publicKey) {
			if (!bytes.IsEmpty) {
				publicKey = null;
				return false;
			}
			publicKey = new BurnPublicKey();
			return true;
		}

		public bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out IPrivateKey privateKey) {
			if (!bytes.IsEmpty) {
				privateKey = null;
				return false;
			}
			privateKey = new BurnPrivateKey();
			return true;
		}

		public IPrivateKey CreatePrivateKey(ReadOnlySpan<byte> secret256) {
			throw new NotImplementedException();
		}

		public IPublicKey DerivePublicKey(IPrivateKey privateKey, ulong signerNonce) {
			throw new NotImplementedException();
		}

		public bool IsPublicKey(IPrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes) {
			return false;
		}

		public byte[] GenerateSalt(ReadOnlySpan<byte> message) {
			throw new NotSupportedException();
		}

		public ReadOnlySpan<byte> ExtractSaltFromSignature(ReadOnlySpan<byte> signature) {
			throw new NotSupportedException();
		}

		public byte[] CalculateMessageDigest(ReadOnlySpan<byte> message, ReadOnlySpan<byte> salt) {
			throw new NotSupportedException();
		}

		public byte[] SignDigest(IPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> salt, ulong signerNonce) {
			throw new NotSupportedException();
		}

		public bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey) {
			return false;
		}

		public class BurnPrivateKey : IPrivateKey {
			public byte[] RawBytes => new byte[0];
		}

		public class BurnPublicKey : IPublicKey {
			public byte[] RawBytes => new byte[0];
		}

	}

}