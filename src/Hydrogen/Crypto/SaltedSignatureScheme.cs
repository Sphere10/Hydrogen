// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.CompilerServices;

namespace Hydrogen;

public class SaltedSignatureScheme<TPrivateKey, TPublicKey, TScheme> : DigitalSignatureSchemeDecorator<TPrivateKey, TPublicKey, TScheme>
	where TPrivateKey : IPrivateKey
	where TPublicKey : IPublicKey
	where TScheme : IDigitalSignatureScheme {
	private readonly int _saltSizeBytes;

	public SaltedSignatureScheme(CHF saltingCHF, int saltSizeBytes, TScheme internalScheme)
		: base(internalScheme) {
		SaltingCHF = saltingCHF;
		_saltSizeBytes = saltSizeBytes;
	}
	public CHF SaltingCHF { get; }

	public override DigitalSignatureSchemeTraits Traits => Internal.Traits & DigitalSignatureSchemeTraits.Salted;

	public virtual byte[] GenerateSalt(ReadOnlySpan<byte> message)
		=> Tools.Crypto.GenerateCryptographicallyRandomBytes(_saltSizeBytes);

	public virtual byte[] JoinSalt(ReadOnlySpan<byte> other, ReadOnlySpan<byte> salt)
		=> Tools.Array.Concat(other, salt);

	public virtual void SplitSalt(ReadOnlySpan<byte> saltedSignature, out ReadOnlySpan<byte> other, out ReadOnlySpan<byte> salt) {
		other = saltedSignature.Slice(0, saltedSignature.Length - _saltSizeBytes);
		salt = saltedSignature.Slice(^_saltSizeBytes);
	}

	public override byte[] CalculateMessageDigest(ReadOnlySpan<byte> message) {
		var salt = GenerateSalt(message);
		var messageDigest = SMAC(message, salt);
		// Note: this returns a salted SMAC. The salt is appended to the end of the SMAC
		// so signature/verification can extract/use it.
		return JoinSalt(messageDigest, salt);
	}

	public override byte[] SignDigest(IPrivateKey privateKey, ReadOnlySpan<byte> saltedSMAC, ulong signerNonce) {
		Guard.ArgumentNotNull(privateKey, nameof(privateKey));
		SplitSalt(saltedSMAC, out var smac, out var salt);
		var signature = Internal.SignDigest(privateKey, smac, signerNonce);
		var saltedSignature = JoinSalt(signature, salt);
		return saltedSignature;
	}

	public override bool VerifyDigest(ReadOnlySpan<byte> saltedSignature, ReadOnlySpan<byte> saltedSMAC, ReadOnlySpan<byte> publicKey) {
		SplitSalt(saltedSMAC, out var messageDigest, out var saltInSMAC);
		SplitSalt(saltedSignature, out var signature, out var saltInSignature);
		if (!saltInSMAC.SequenceEqual(saltInSignature))
			return false;
		return Internal.VerifyDigest(signature, messageDigest, publicKey);
	}


	/// <summary>
	/// Returns an SMAC of the argument.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="seed"></param>
	/// <returns>H(seed || H(seed || H(message)))</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte[] SMAC(ReadOnlySpan<byte> message, ReadOnlySpan<byte> seed)
		=> HMAC(Internal.CalculateMessageDigest(message), seed);

	/// <summary>
	/// Returns an HMAC of the argument.
	/// </summary>
	/// <param name="digest"></param>
	/// <param name="seed"></param>
	/// <returns>H(seed || H(seed || digest))</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte[] HMAC(ReadOnlySpan<byte> digest, ReadOnlySpan<byte> seed) {
		using (Hashers.BorrowHasher(SaltingCHF, out var hasher)) {
			hasher.Transform(seed);
			hasher.Transform(digest);
			var innerHash = hasher.GetResult();
			hasher.Transform(seed);
			hasher.Transform(innerHash);
			return hasher.GetResult();
		}
	}


}
