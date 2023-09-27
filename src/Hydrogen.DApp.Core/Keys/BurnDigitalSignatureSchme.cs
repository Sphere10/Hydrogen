// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.DApp.Core.Keys;

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
