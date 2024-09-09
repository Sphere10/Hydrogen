// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public abstract class DigitalSignatureSchemeBase<TPrivateKey, TPublicKey> : IDigitalSignatureScheme
	where TPrivateKey : IPrivateKey
	where TPublicKey : IPublicKey {


	protected DigitalSignatureSchemeBase(CHF messageDigestCHF) {
		MessageDigestCHF = messageDigestCHF;
		Traits = DigitalSignatureSchemeTraits.None;
	}

	public CHF MessageDigestCHF { get; }

	public int MessageDigestLength => Hashers.GetDigestSizeBytes(MessageDigestCHF);

	public DigitalSignatureSchemeTraits Traits { get; protected set; }

	public abstract IIESAlgorithm IES { get; }
	
	

	public abstract bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out TPublicKey publicKey);

	public abstract bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out TPrivateKey privateKey);

	public TPrivateKey GeneratePrivateKey()
		=> (TPrivateKey)((IDigitalSignatureScheme)this).GeneratePrivateKey();

	public abstract TPrivateKey GeneratePrivateKey(ReadOnlySpan<byte> seed);

	public abstract TPublicKey DerivePublicKey(TPrivateKey privateKey, ulong signerNonce);

	public abstract bool IsPublicKey(TPrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes);

	public virtual byte[] CalculateMessageDigest(ReadOnlySpan<byte> message)
		=> Hashers.Hash(MessageDigestCHF, message);

	public abstract byte[] SignDigest(TPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ulong signerNonce);

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

	byte[] IDigitalSignatureScheme.SignDigest(IPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ulong signerNonce)
		=> SignDigest((TPrivateKey)privateKey, messageDigest, signerNonce);


}
