// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Extension methods for all types.
/// </summary>
/// <remarks></remarks>
public static class UniversalExtensions {

	public static SignedItem<TItem> AsSignedItem<TItem>(this TItem item, IItemSerializer<TItem> serializer, CHF chf, DSS dss, byte[] privateKey, ulong signerNonce = 0, Endianness endianness = HydrogenDefaults.Endianness) {
		var scheme = Signers.Create(dss);
		var key = scheme.ParsePrivateKey(privateKey);
		var digestor = new ItemDigestor<TItem>(chf, serializer, endianness);
		var signer = new ItemSigner<TItem>(digestor, scheme);
		return AsSignedItem(item, signer, key, signerNonce);
	}

	public static SignedItem<TItem> AsSignedItem<TItem>(this TItem item, IItemHasher<TItem> digestor, IDigitalSignatureScheme dss, IPrivateKey privateKey, ulong signerNonce = 0)
		=> AsSignedItem(item, new ItemSigner<TItem>(digestor, dss), privateKey, signerNonce);

	public static SignedItem<TItem> AsSignedItem<TItem>(this TItem item, IItemSigner<TItem> signer, IPrivateKey privateKey, ulong signerNonce = 0)
		=> new() {
			Item = item,
			Signature = signer.Sign(item, privateKey, signerNonce)
		};

	public static IEnumerable<TItem> Visit<TItem>(this TItem node, Func<TItem, TItem> ancestorIterator, Func<TItem, bool> discriminator = null, IEqualityComparer<TItem> comparer = null)
		=> Visit(node, x => x != null ? [ancestorIterator(x)] : Array.Empty<TItem>(), discriminator, comparer);

	public static IEnumerable<TItem> Visit<TItem>(this TItem node, Func<TItem, IEnumerable<TItem>> edgeIterator, Func<TItem, bool> discriminator = null, IEqualityComparer<TItem> comparer = null)
		=> new[] { node }.Visit(edgeIterator, discriminator, comparer);

	public static void Swap<T>(ref T fromX, ref T fromY) {
		T temp = default(T);
		temp = fromX;
		fromX = fromY;
		fromY = temp;
	}

	public static bool IsIn<T>(this T @object, params T[] collection) {
		return collection.Contains(@object);
	}

	public static bool IsIn<T>(this T @object, IEnumerable<T> collection) {
		return collection.Contains(@object);
	}

	//public static string ToStringSafe<T>(this T @object) => @object?.ToString() ?? "<null>";


	public static IEnumerable<T> ConcatWith<T>(this T head, IEnumerable<T> tail) => new[] { head }.Concat(tail);

	public static IEnumerable<T> UnionWith<T>(this T head, IEnumerable<T> tail) => new[] { head }.Union(tail);

	public static IEnumerable<T> UnionWith<T>(this T head, T tail) => UnionWith(head, new[] { tail });

}
