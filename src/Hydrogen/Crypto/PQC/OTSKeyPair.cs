// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class OTSKeyPair {
	public OTSKeyPair(byte[,] privateKey, byte[,] publicKey, IFuture<byte[]> publicKeyHash) {
		PrivateKey = privateKey;
		PublicKey = publicKey;
		PublicKeyHash = publicKeyHash;
	}
	public readonly byte[,] PrivateKey;
	public readonly byte[,] PublicKey;
	public readonly IFuture<byte[]> PublicKeyHash;
}
