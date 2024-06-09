// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Runtime.CompilerServices;
namespace Hydrogen;

public class ItemDigestor<TItem> : ItemSerializerDecorator<TItem>, IItemHasher<TItem>, IItemChecksummer<TItem> {
	private readonly CHF _hashAlgorithm;
	private readonly Endianness _endianness;
	private readonly HashChecksummer _hashChecksummer;

	public ItemDigestor(IItemSerializer<TItem> internalSerializer, Endianness endianness = HydrogenDefaults.Endianness)
		: this(CHF.SHA2_256, internalSerializer, endianness) {
	}

	public ItemDigestor(CHF hashAlgorithm, IItemSerializer<TItem> internalSerializer = null, Endianness endianness = HydrogenDefaults.Endianness)
		: base(internalSerializer ?? ItemSerializer<TItem>.Default) {
		_hashAlgorithm = hashAlgorithm;
		_endianness = endianness;
		_hashChecksummer = new HashChecksummer();
		DigestLength = Hashers.GetDigestSizeBytes(_hashAlgorithm);
	}

	public int DigestLength { get; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte[] Hash(TItem item) => Hashers.Hash(_hashAlgorithm, item, this, _endianness);

	public int CalculateChecksum(TItem item) => _hashChecksummer.CalculateChecksum(Hash(item));
}
