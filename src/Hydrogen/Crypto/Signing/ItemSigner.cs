﻿using System;

namespace Hydrogen;

public class ItemSigner<TItem> : IItemSigner<TItem> {
	
	public ItemSigner(IItemSerializer<TItem> serializer, CHF chf, DSS dss, Endianness endianness = Endianness.LittleEndian) 
		: this(new ItemDigestor<TItem>(chf, serializer, endianness), Signers.Create(dss)) {
	}
	
	public ItemSigner(IItemDigestor<TItem> digestor, DSS dss) 
		: this(digestor, Signers.Create(dss)) {
	}

	public ItemSigner(IItemDigestor<TItem> digestor, IDigitalSignatureScheme digitalSignatureScheme) {
		Digestor = digestor;
		DigitalSignatureScheme = digitalSignatureScheme;
	}

	public IItemDigestor<TItem> Digestor { get; }

	public IDigitalSignatureScheme DigitalSignatureScheme { get; }

	public bool TrySign(TItem item, IPrivateKey privateKey, ulong signerNonce, out byte[] signature) {
		var digest = Digestor.Hash(item);
		signature = DigitalSignatureScheme.SignDigest(privateKey, digest, signerNonce);
		return true;
	}

	public bool TryVerify(TItem item, IPublicKey publicKey, byte[] signature, out bool isValidSignature) {
		var digest = Digestor.Hash(item);
		isValidSignature = DigitalSignatureScheme.VerifyDigest(signature, digest, publicKey);
		return true;
	}
}