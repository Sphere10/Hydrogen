// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace Hydrogen;

public abstract record SignedItem {

	[XmlElement("Item")]
	[DataMember(Name = "item", EmitDefaultValue = true)]
	public object Item { get; set; }

	[XmlElement("Signature")]
	[DataMember(Name = "signature", EmitDefaultValue = true)]
	public byte[] Signature { get; set; }

}


public record SignedItem<T> : SignedItem {

	[XmlElement("Item")]
	[DataMember(Name = "item", EmitDefaultValue = true)]
	public new T Item {
		get => (T)base.Item;
		set => base.Item = value;
	}

	public bool Verify(IItemSerializer<T> serializer, CHF chf, DSS dss, byte[] publicKey, ulong signerNonce = 0, Endianness endianness = HydrogenDefaults.Endianness)
		=> Verify(new ItemSigner<T>(new ItemDigestor<T>(chf, serializer, endianness), dss), publicKey);

	public bool Verify(IItemHasher<T> digestor, IDigitalSignatureScheme dss, IPublicKey publicKey)
		=> Verify(new ItemSigner<T>(digestor, dss), publicKey);

	public bool Verify(IItemSigner<T> signer, byte[] publicKey)
		=> Verify(signer, signer.DigitalSignatureScheme.ParsePublicKey(publicKey));

	public bool Verify(IItemSigner<T> signer, IPublicKey publicKey)
		=> signer.Verify(Item, publicKey, Signature);

}
