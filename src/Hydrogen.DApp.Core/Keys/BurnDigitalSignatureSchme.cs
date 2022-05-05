using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Hydrogen;
using Hydrogen.DApp.Core.Maths;

namespace Hydrogen.DApp.Core.Keys {

	public sealed class BurnDigitalSignatureSchme : StatelessDigitalSignatureScheme<BurnDigitalSignatureSchme.NoOpKey, BurnDigitalSignatureSchme.NoOpKey> {
		
		public BurnDigitalSignatureSchme(CHF messageDigestCHF) : base(messageDigestCHF) {
			Traits = DigitalSignatureSchemeTraits.None;
		}

		public override IIESAlgorithm IES => throw new NotSupportedException();


		public override bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out NoOpKey publicKey) {
			throw new NotSupportedException();
		}

		public override bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out NoOpKey privateKey) {
			throw new NotSupportedException();
		}

		public override NoOpKey GeneratePrivateKey(ReadOnlySpan<byte> seed) {
			throw new NotSupportedException();
		}

		public override bool IsPublicKey(NoOpKey privateKey, ReadOnlySpan<byte> publicKeyBytes) {
			throw new NotSupportedException();
		}


		public override bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey) {
			throw new NotSupportedException();
		}


		public override NoOpKey DerivePublicKey(NoOpKey privateKey) {
			throw new NotSupportedException();
		}

		public override byte[] SignDigest(NoOpKey privateKey, ReadOnlySpan<byte> messageDigest) {
			throw new NotSupportedException();
		}

		public bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out IPublicKey publicKey) {
			if (!bytes.IsEmpty) {
				publicKey = null;
				return false;
			}
			publicKey = new NoOpKey();
			return true;
		}

		public bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out IPrivateKey privateKey) {
			if (!bytes.IsEmpty) {
				privateKey = null;
				return false;
			}
			privateKey = new NoOpKey();
			return true;
		}


		
		public class NoOpKey : IPrivateKey, IPublicKey {
			public byte[] RawBytes { get; }
		}



	}

}