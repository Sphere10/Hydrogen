// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public interface IItemSigner<in TItem> {

	IDigitalSignatureScheme DigitalSignatureScheme { get; }

	bool TrySign(TItem item, IPrivateKey privateKey, ulong signerNonce, out byte[] signature);

	bool TryVerify(TItem item, IPublicKey publicKey, byte[] signature, out bool isValidSignature);
}


public static class ItemSignerExtensions {
	public static byte[] Sign<T>(this IItemSigner<T> signer, T item, IPrivateKey privateKey, ulong signerNonce) {
		if (!signer.TrySign(item, privateKey, signerNonce, out var signature))
			throw new InvalidOperationException($"Unable to sign item: {item}");
		return signature;
	}

	public static bool Verify<T>(this IItemSigner<T> signer, T item, IPublicKey publicKey, byte[] signature) {
		if (!signer.TryVerify(item, publicKey, signature, out var isValid))
			throw new InvalidOperationException($"Unable to verify item signature: {item}");
		return isValid;
	}
}
