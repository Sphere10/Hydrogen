// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Hydrogen.ObjectSpaces;

namespace Hydrogen;

/// <summary>
/// A list whose items are persisted over a stream via an <see cref="ObjectStreamStream{T}"/>.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class StreamMappedList<TItem> : SingularListBase<TItem>, IStreamMappedList<TItem> {
	public event EventHandlerEx<object> Loading { add => ObjectStream.Loading += value; remove => ObjectStream.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => ObjectStream.Loaded += value; remove => ObjectStream.Loaded -= value; }

	private readonly ProjectionIndex<TItem, int> _projectionChecksumIndex;

	internal StreamMappedList(
		ObjectStream<TItem> objectStream,
		string optionalItemChecksumIndexName = null,
		IEqualityComparer<TItem> itemComparer = null,
		bool autoLoad = false
	) {
		Guard.ArgumentNotNull(objectStream, nameof(objectStream));
		ObjectStream = objectStream;
		ItemComparer = itemComparer ?? EqualityComparer<TItem>.Default;

		Guard.Against(!string.IsNullOrEmpty(optionalItemChecksumIndexName) && !objectStream.Streams.Attachments.ContainsKey(optionalItemChecksumIndexName), $"No index '{optionalItemChecksumIndexName}' was found in object stream"); 
		if (!string.IsNullOrEmpty(optionalItemChecksumIndexName)) 
			_projectionChecksumIndex = (ProjectionIndex<TItem, int>)objectStream.Streams.Attachments[optionalItemChecksumIndexName]; // use for O(1) exists
		
		if (autoLoad && RequiresLoad)
			Load();
	}

	public override long Count => ObjectStream.Count;

	public ObjectStream<TItem> ObjectStream { get; }

	ObjectStream IStreamMappedCollection.ObjectStream => ObjectStream;

	public IItemSerializer<TItem> ItemSerializer => ObjectStream.ItemSerializer;

	public IEqualityComparer<TItem> ItemComparer { get; }

	public bool OwnsContainer { get; set; }

	public virtual bool RequiresLoad => ObjectStream.RequiresLoad;

	public virtual void Load() => ObjectStream.Load();

	public virtual Task LoadAsync() => ObjectStream.LoadAsync();

	public virtual void Dispose() {
		if (OwnsContainer)
			ObjectStream.Dispose();
	}

	public override TItem Read(long index) {
		CheckIndex(index, true);
		return ObjectStream.LoadItem(index);
	}

	public override long IndexOfL(TItem item) {
		var indicesToCheck =
			_projectionChecksumIndex != null ?
			_projectionChecksumIndex.Values[_projectionChecksumIndex.ApplyProjection(item)] :
			Tools.Collection.RangeL(0L, Count);

		foreach (var index in indicesToCheck) {
			if (ItemComparer.Equals(item, Read(index)))
				return index;
		}
		return -1L;
	}

	public override bool Contains(TItem item) => IndexOf(item) != -1;

	public override void Add(TItem item) {
		using var _ = EnterAddScope(item);
	}

	public override void Insert(long index, TItem item) {
		CheckIndex(index, true);
		using var _ = EnterInsertScope(index, item);
	}

	public override void Update(long index, TItem item) {
		CheckIndex(index, false);
		using var _ = EnterUpdateScope(index, item);
	}

	public override bool Remove(TItem item) {
		var index = IndexOf(item);
		if (index >= 0) {
			UpdateVersion();
			ObjectStream.RemoveItem(index);
			return true;
		}
		return false;
	}

	public override void RemoveAt(long index) {
		CheckIndex(index, false);
		UpdateVersion();
		ObjectStream.RemoveItem(index);
	}

	public override void Clear() {
		UpdateVersion();
		ObjectStream.Clear();
	}

	public override void CopyTo(TItem[] array, int arrayIndex) {
		Guard.ArgumentNotNull(array, nameof(array));
		Guard.ArgumentInRange(arrayIndex, 0, Math.Max(0, array.Length - 1), nameof(array));
		var itemsToCopy = Math.Min(Count, array.Length - arrayIndex);
		for (var i = 0; i < itemsToCopy; i++)
			array[i + arrayIndex] = Read(i);
	}

	public override IEnumerator<TItem> GetEnumerator() {
		var version = Version;
		var count = Count;
		for (var i = 0; i < count; i++) {
			CheckVersion(version);
			yield return Read(i);
		}
	}

	protected ClusteredStream EnterAddScope(TItem item) {
		// Index checking deferred to Streams
		UpdateVersion();
		return ObjectStream.SaveItemAndReturnStream(ObjectStream.Count, item, ObjectStreamOperationType.Add);
	}

	protected ClusteredStream EnterInsertScope(long index, TItem item) {
		// Index checking deferred to Streams
		UpdateVersion();
		return ObjectStream.SaveItemAndReturnStream(index, item, ObjectStreamOperationType.Insert);
	}

	protected ClusteredStream EnterUpdateScope(long index, TItem item) {
		// Index checking deferred to Streams
		UpdateVersion();
		return ObjectStream.SaveItemAndReturnStream(index, item, ObjectStreamOperationType.Update);
	}
	
}
