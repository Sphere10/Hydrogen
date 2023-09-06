// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A set whose items are mapped over a stream as a <see cref="StreamMappedList{TItem}"/>. A digest of the items are kept in the clustered record for fast lookup. 
///
/// </summary>
/// <remarks>When deleting an item the underlying <see cref="ClusteredStreamDescriptor"/> is marked nullified but retained and re-used in later calls to <see cref="Add(TItem)"/>.</remarks>
public class StreamMappedHashSet<TItem> : SetBase<TItem>, IStreamMappedHashSet<TItem> {
	public event EventHandlerEx<object> Loading {
		add => InternalDictionary.Loading += value;
		remove => InternalDictionary.Loading -= value;
	}

	public event EventHandlerEx<object> Loaded {
		add => InternalDictionary.Loaded += value;
		remove => InternalDictionary.Loaded -= value;
	}

	internal readonly IStreamMappedDictionary<byte[], TItem> InternalDictionary;
	private readonly IItemHasher<TItem> _hasher;

	public StreamMappedHashSet(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TItem> serializer,
		CHF chf,
		IEqualityComparer<TItem> comparer = null,
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		Endianness endianness = Endianness.LittleEndian)
		: this(
			  rootStream,
			  clusterSize,
			  serializer,
			  new ItemDigestor<TItem>(chf, serializer),
			  comparer,
			  policy,
			  endianness
		) {
	}

	public StreamMappedHashSet(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TItem> serializer,
		IItemHasher<TItem> hasher,
		IEqualityComparer<TItem> comparer = null,
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		Endianness endianness = Endianness.LittleEndian
	) : this(
			new StreamMappedDictionaryCLK<byte[], TItem>(
				rootStream,
				clusterSize,
				new StaticSizeByteArraySerializer(hasher.DigestLength).AsNullable(),
				serializer,
				new ByteArrayEqualityComparer(),
				comparer,
				policy,
				endianness),
			comparer,
			hasher
		) {
	}

	public StreamMappedHashSet(
		IStreamMappedDictionary<byte[], TItem> internalDictionary,
		IEqualityComparer<TItem> comparer,
		IItemHasher<TItem> hasher
	) : base(comparer ?? EqualityComparer<TItem>.Default) {
		Guard.ArgumentNotNull(internalDictionary, nameof(internalDictionary));
		InternalDictionary = internalDictionary;
		_hasher = hasher;
	}

	public override int Count => InternalDictionary.Count;

	public override bool IsReadOnly => InternalDictionary.IsReadOnly;

	public bool RequiresLoad => InternalDictionary.RequiresLoad;

	public ObjectContainer ObjectContainer => InternalDictionary.ObjectContainer;

	public void Load() => InternalDictionary.Load();

	public Task LoadAsync() => Task.Run(Load);

	public override bool Add(TItem item) {
		Guard.ArgumentNotNull(item, nameof(item));
		var itemHash = _hasher.Hash(item);
		if (InternalDictionary.ContainsKey(itemHash))
			return false;
		InternalDictionary.Add(itemHash, item);
		return true;
	}

	public override bool Contains(TItem item) {
		Guard.ArgumentNotNull(item, nameof(item));
		return InternalDictionary.ContainsKey(_hasher.Hash(item));
	}

	public override bool Remove(TItem item) {
		Guard.ArgumentNotNull(item, nameof(item));
		var itemHash = _hasher.Hash(item);
		if (!InternalDictionary.TryFindKey(itemHash, out var index))
			return false;
		InternalDictionary.RemoveAt(index);
		return true;
	}

	public override void Clear()
		=> InternalDictionary.Clear();

	public override void CopyTo(TItem[] array, int arrayIndex)
		=> InternalDictionary.Values.CopyTo(array, arrayIndex);

	public override IEnumerator<TItem> GetEnumerator()
		=> InternalDictionary.Values.GetEnumerator();

	public void Dispose() => InternalDictionary.Dispose();
}
