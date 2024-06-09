// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;


namespace Hydrogen;

public class Signers {
	private static readonly Func<IDigitalSignatureScheme>[] Factories;

	static Signers() {
		Factories = new Func<IDigitalSignatureScheme>[byte.MaxValue];
		RegisterDefaultAlgorithms();
	}

	// NOTE: attack vector if malicious plug-in library plugs in a man-in-the-middle sniffer to learn private details
	// MUST ACTION!
	public static void Register(DSS algorithm, Func<IDigitalSignatureScheme> constructor) {
		Factories[(byte)algorithm] = constructor;
	}

	public static IDigitalSignatureScheme Create(DSS scheme) {
		var factory = Factories[(byte)scheme];
		Guard.Ensure(factory is not null, $"No implementation for {scheme} was found");
		return factory();
	}

	public static bool TryParsePublicKey(DSS dss, ReadOnlySpan<byte> bytes, out IPublicKey publicKey)
		=> Create(dss).TryParsePublicKey(bytes, out publicKey);

	public static IPublicKey ParsePublicKey(DSS dss, ReadOnlySpan<byte> bytes)
		=> Create(dss).ParsePublicKey(bytes);

	public static bool TryParsePrivateKey(DSS dss, ReadOnlySpan<byte> bytes, out IPrivateKey privateKey)
		=> Create(dss).TryParsePrivateKey(bytes, out privateKey);

	public static IPrivateKey ParsePrivateKey(DSS dss, ReadOnlySpan<byte> bytes)
		=> Create(dss).ParsePrivateKey(bytes);

	public static IPrivateKey CreatePrivateKey(DSS dss, ReadOnlySpan<byte> secret256)
		=> Create(dss).CreatePrivateKey(secret256);

	public static IPrivateKey GeneratePrivateKey(DSS dss)
		=> Create(dss).GeneratePrivateKey();

	public static IPublicKey DerivePublicKey(DSS dss, IPrivateKey privateKey, ulong signerNonce)
		=> Create(dss).DerivePublicKey(privateKey, signerNonce);

	public static bool IsPublicKey(DSS dss, IPrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes)
		=> Create(dss).IsPublicKey(privateKey, publicKeyBytes);

	public static byte[] CalculateMessageDigest(DSS dss, ReadOnlySpan<byte> message)
		=> Create(dss).CalculateMessageDigest(message);

	public static byte[] SignDigest(DSS dss, IPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ulong signerNonce)
		=> Create(dss).SignDigest(privateKey, messageDigest, signerNonce);

	public static byte[] Sign(DSS dss, IPrivateKey privateKey, ReadOnlySpan<byte> message, ulong signerNonce)
		=> Create(dss).Sign(privateKey, message, signerNonce);

	public static bool VerifyDigest(DSS dss, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey)
		=> Create(dss).VerifyDigest(signature, messageDigest, publicKey);

	public static bool VerifyDigest(DSS dss, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, IPublicKey publicKey)
		=> Create(dss).VerifyDigest(signature, messageDigest, publicKey);

	public static bool Verify(DSS dss, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> message, IPublicKey publicKey)
		=> Create(dss).Verify(signature, message, publicKey);

	public static bool Verify(DSS dss, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> message, ReadOnlySpan<byte> publicKey)
		=> Create(dss).Verify(signature, message, publicKey);

	public static void RegisterDefaultAlgorithms() {
		Register(DSS.PQC_WAMS, () => new WAMS());
		Register(DSS.PQC_WAMSSharp, () => new WAMSSharp());
		// Rest of DSS's are registered via Hydrogen.CryptoEx ModuleConfiguration
	}

}
