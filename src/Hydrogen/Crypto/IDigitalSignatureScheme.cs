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

public interface IDigitalSignatureScheme {
	DigitalSignatureSchemeTraits Traits { get; }
	
	IIESAlgorithm IES { get; }

	public int MessageDigestLength { get; }

	bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out IPublicKey publicKey);

	bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out IPrivateKey privateKey);

	IPrivateKey CreatePrivateKey(ReadOnlySpan<byte> secret256);

	IPublicKey DerivePublicKey(IPrivateKey privateKey, ulong signerNonce);

	bool IsPublicKey(IPrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes);

	byte[] CalculateMessageDigest(ReadOnlySpan<byte> message);

	byte[] SignDigest(IPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ulong signerNonce);

	bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey);

}


public static class IDigitalSignatureSchemeExtensions {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IPrivateKey GeneratePrivateKey(this IDigitalSignatureScheme dss)
		=> dss.CreatePrivateKey(Tools.Crypto.GenerateCryptographicallyRandomBytes(32));

	public static IPublicKey ParsePublicKey(this IDigitalSignatureScheme dss, ReadOnlySpan<byte> bytes) {
		if (!dss.TryParsePublicKey(bytes, out var key))
			throw new ArgumentException("Not a valid public key", nameof(bytes));
		return key;
	}

	public static IPrivateKey ParsePrivateKey(this IDigitalSignatureScheme dss, ReadOnlySpan<byte> bytes) {
		if (!dss.TryParsePrivateKey(bytes, out var key))
			throw new ArgumentException("Not a valid private key", nameof(bytes));
		return key;
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] Sign(this IDigitalSignatureScheme dss, IPrivateKey privateKey, ReadOnlySpan<byte> message, ulong signerNonce) {
		var messageDigest = dss.CalculateMessageDigest(message);
		return dss.SignDigest(privateKey, messageDigest, signerNonce);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Verify(this IDigitalSignatureScheme dss, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> message, IPublicKey publicKey)
		=> dss.Verify(signature, message, publicKey.RawBytes);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Verify(this IDigitalSignatureScheme dss, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> message, ReadOnlySpan<byte> publicKey) {
		var messageDigest = dss.CalculateMessageDigest(message);
		return dss.VerifyDigest(signature, messageDigest, publicKey);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool VerifyDigest(this IDigitalSignatureScheme dss, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, IPublicKey publicKey)
		=> dss.VerifyDigest(signature, messageDigest, publicKey.RawBytes);

}
