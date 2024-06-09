// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hydrogen.ObjectSpaces;

namespace Hydrogen;

/// <summary>
/// A set whose items are mapped over a stream as a <see cref="StreamMappedList{TItem}"/>. A digest of the items are kept in the clustered record for fast lookup. 
///
/// </summary>
/// <remarks>When deleting an item the underlying <see cref="ClusteredStreamDescriptor"/> is marked nullified but retained and re-used in later calls to <see cref="Add(TItem)"/>.</remarks>
public class StreamMappedHashSet<TItem> : SetBase<TItem>, IStreamMappedHashSet<TItem> {
	public event EventHandlerEx<object> Loading { add => InternalDictionary.Loading += value; remove => InternalDictionary.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalDictionary.Loaded += value; remove => InternalDictionary.Loaded -= value; }

	internal readonly IStreamMappedDictionary<byte[], TItem> InternalDictionary;
	private readonly IItemHasher<TItem> _hasher;

	internal StreamMappedHashSet(IStreamMappedDictionary<byte[], TItem> internalDictionary, IEqualityComparer<TItem> comparer, IItemHasher<TItem> hasher) 
		: base(comparer ?? EqualityComparer<TItem>.Default) {
		Guard.ArgumentNotNull(internalDictionary, nameof(internalDictionary));
		InternalDictionary = internalDictionary;
		_hasher = hasher;
	}

	public override int Count => InternalDictionary.Count;

	public override bool IsReadOnly => InternalDictionary.IsReadOnly;

	public bool RequiresLoad => InternalDictionary.RequiresLoad;

	public ObjectStream ObjectStream => InternalDictionary.ObjectStream;

	public void Load() => InternalDictionary.Load();

	public Task LoadAsync() => Task.Run(Load);

	public override bool Add(TItem item) {
		Guard.ArgumentNotNull(item, nameof(item));
		using (ObjectStream.EnterAccessScope()) {
			var itemHash = _hasher.Hash(item);
			if (InternalDictionary.ContainsKey(itemHash))
				return false;
			InternalDictionary.Add(itemHash, item);
			return true;
		}
	}

	public override bool Contains(TItem item) {
		Guard.ArgumentNotNull(item, nameof(item));
		return InternalDictionary.ContainsKey(_hasher.Hash(item));
	}

	public override bool Remove(TItem item) {
		Guard.ArgumentNotNull(item, nameof(item));
		using (ObjectStream.EnterAccessScope()) {
			var itemHash = _hasher.Hash(item);
			if (!InternalDictionary.TryFindKey(itemHash, out var index))
				return false;
			InternalDictionary.RemoveAt(index);
			return true;
		}
	}

	public override void Clear()
		=> InternalDictionary.Clear();

	public override void CopyTo(TItem[] array, int arrayIndex)
		=> InternalDictionary.Values.CopyTo(array, arrayIndex);

	public override IEnumerator<TItem> GetEnumerator()
		=> InternalDictionary.Values.GetEnumerator();

	public void Dispose() => InternalDictionary.Dispose();
}
