// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hydrogen.ObjectSpaces;

namespace Hydrogen;

/// <summary>
/// A dictionary whose contents are mapped onto a stream using an <see cref="ObjectStream"/> which in turn uses a <see cref="ClusteredStreams"/>.
/// This implementation persists <see cref="KeyValuePair{TKey, TValue}"/>s to a stream and uses an index on the <see cref="TKey"/> checksum to find keys
/// in the objectStream. This is suitable for general purpose dictionaries whose key's of arbitrary length. For keys that are constant length, a more
/// optimized version is <see cref="StreamMappedDictionaryCLK{TKey,TValue}"/>. 
/// </summary>
/// <typeparam name="TKey">The type of key stored in the dictionary</typeparam>
/// <typeparam name="TValue">The type of value stored in the dictionary</typeparam>
/// <remarks>When deleting an item the underlying slot in the <see cref="ObjectStream"/> is marked as reaped and that index re-used in subsequent <see cref="Add(TKey,TValue)"/> operations.</remarks>
public class StreamMappedDictionary<TKey, TValue> : DictionaryBase<TKey, TValue>, IStreamMappedDictionary<TKey, TValue> {
	public event EventHandlerEx<object> Loading { add => ObjectStream.Loading += value; remove => ObjectStream.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => ObjectStream.Loaded += value; remove => ObjectStream.Loaded -= value; }

	private readonly IEqualityComparer<TValue> _valueComparer;
	private readonly ProjectionChecksumIndex<KeyValuePair<TKey, TValue>, TKey> _projectionChecksumIndex;
	private readonly RecyclableIndexIndex _recyclableIndexStore;

	internal StreamMappedDictionary(ObjectStream objectStream, string recyclableIndexName, string keyChecksumIndexName, IEqualityComparer<TValue> valueComparer = null, bool autoLoad = false) {
		Guard.ArgumentNotNull(objectStream, nameof(objectStream));
		Guard.ArgumentIsAssignable<ObjectStream<KeyValuePair<TKey, TValue>>>(objectStream, nameof(objectStream));
		ObjectStream = (ObjectStream<KeyValuePair<TKey, TValue>>)objectStream;
		_recyclableIndexStore = (RecyclableIndexIndex)objectStream.Streams.Attachments[recyclableIndexName];
		_projectionChecksumIndex = (ProjectionChecksumIndex<KeyValuePair<TKey, TValue>, TKey>)objectStream.Streams.Attachments[keyChecksumIndexName];
		_valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
		
		if (autoLoad && RequiresLoad) 
			Load();
	}


	ObjectStream IStreamMappedCollection.ObjectStream => ObjectStream;

	public ObjectStream<KeyValuePair<TKey, TValue>> ObjectStream { get; }

	protected IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs => GetEnumerator().AsEnumerable();

	public bool RequiresLoad => ObjectStream.RequiresLoad;

	public override long Count {
		get {
			CheckLoaded();
			return ObjectStream.Count - _recyclableIndexStore.Store.Count;
		}
	}

	public override bool IsReadOnly => false;

	public bool OwnsContainer { get; set; }

	public void Load() => ObjectStream.Load();

	public Task LoadAsync() => ObjectStream.LoadAsync();

	public virtual void Dispose() {
		if (OwnsContainer)
			ObjectStream.Dispose();
	}

	public TKey ReadKey(long index) {
		using (ObjectStream.EnterAccessScope()) {
			var traits = ObjectStream.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");

			using var stream = ObjectStream.Streams.OpenRead(ObjectStream.Streams.Header.ReservedStreams + index);
			var reader = new EndianBinaryReader(EndianBitConverter.For(ObjectStream.Streams.Endianness), stream);
			return ((KeyValuePairSerializer<TKey, TValue>)ObjectStream.ItemSerializer).DeserializeKey(reader);
		}
	}

	public byte[] ReadKeyBytes(long index) {
		using (ObjectStream.EnterAccessScope()) {
			var traits = ObjectStream.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");

			using var stream = ObjectStream.Streams.OpenRead(ObjectStream.Streams.Header.ReservedStreams + index);
			var reader = new EndianBinaryReader(EndianBitConverter.For(ObjectStream.Streams.Endianness), stream);
			return ((KeyValuePairSerializer<TKey, TValue>)ObjectStream.ItemSerializer).ReadKeyBytes(reader);
		}
	}

	public TValue ReadValue(long index) {
		using (ObjectStream.EnterAccessScope()) {
			var traits = ObjectStream.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");
			
			using var stream = ObjectStream.Streams.OpenRead(ObjectStream.Streams.Header.ReservedStreams + index);
			var reader = new EndianBinaryReader(EndianBitConverter.For(ObjectStream.Streams.Endianness), stream);
			return ((KeyValuePairSerializer<TKey, TValue>)ObjectStream.ItemSerializer).DeserializeValue(reader);
		}
	}

	public byte[] ReadValueBytes(long index) {
		using (ObjectStream.EnterAccessScope()) {
			var traits = ObjectStream.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");

			using var stream = ObjectStream.Streams.OpenRead(ObjectStream.Streams.Header.ReservedStreams + index);
			var reader = new EndianBinaryReader(EndianBitConverter.For(ObjectStream.Streams.Endianness), stream);
			var bytes = ((KeyValuePairSerializer<TKey, TValue>)ObjectStream.ItemSerializer).ReadValueBytes(reader);
			
			// The value is null, so it's bytes are null (null is serialized as a single byte 0)
			if (bytes.Length == 1 && bytes[0] == 0)
				return null;

			return bytes;
		}
	}

	public override void Add(TKey key, TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			if (TryFindKey(key, out _))
				throw new KeyNotFoundException($"An item with key '{key}' was already added");
			var kvp = new KeyValuePair<TKey, TValue>(key, value);
			AddInternal(kvp);
		}
	}

	public override void Update(TKey key, TValue value) {
		using (ObjectStream.EnterAccessScope()) {
			if (!TryFindKey(key, out var index))
				throw new KeyNotFoundException($"The key '{key}' was not found");
			var kvp = new KeyValuePair<TKey, TValue>(key, value);
			UpdateInternal(index, kvp);
		}
	}

	protected override void AddOrUpdate(TKey key, TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			var kvp = new KeyValuePair<TKey, TValue>(key, value);
			if (TryFindKey(key, out var index)) {
				UpdateInternal(index, kvp);
			} else {
				AddInternal(kvp);
			}
		}
	}

	public override bool Remove(TKey key) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			if (TryFindKey(key, out var index)) {
				RemoveAt(index);
				return true;
			}
			return false;
		}
	}

	public override bool ContainsKey(TKey key) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			return _projectionChecksumIndex.Values[key].Any();
		}
	}

	public override void Clear() {
		// Load not required
		using (ObjectStream.EnterAccessScope()) {
			ObjectStream.Clear();
			UpdateVersion();
		}
	}

	public override bool TryGetValue(TKey key, out TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			if (TryFindValue(key, out _, out value)) {
				return true;
			}
			value = default;
			return false;
		}
	}

	public bool TryFindKey(TKey key, out long index) {
		Debug.Assert(key != null);
		using (ObjectStream.EnterAccessScope()) {
			var matches = _projectionChecksumIndex.Values[key].ToArray();
			if (matches.Length == 0) {
				index = -1;
				return false;
			}
			Guard.Ensure(matches.Length == 1, "Duplicate keys encountered in storage (data corrupt)"); 
			index = matches[0];
			return true;
		}
	}

	public bool TryFindValue(TKey key, out long index, out TValue value) {
		Debug.Assert(key != null);
		using (ObjectStream.EnterAccessScope()) {
			var matches = _projectionChecksumIndex.Values[key].ToArray();
			if (matches.Length == 0) {
				index = -1;
				value = default;
				return false;
			}
			Guard.Ensure(matches.Length == 1, "Duplicate keys encountered in storage (data corrupt)"); 
			index = matches[0];
			value = ReadValue(index);
			return true;
		}
	}

	public override void Add(KeyValuePair<TKey, TValue> item) {
		Guard.ArgumentNotNull(item, nameof(item));
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			if (ContainsKey(item.Key))
				throw new KeyNotFoundException($"An item with key '{item.Key}' was already added");
			AddInternal(item);
		}
	}

	public override bool Remove(KeyValuePair<TKey, TValue> item) {
		Guard.ArgumentNotNull(item, nameof(item));
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			if (TryFindValue(item.Key, out var index, out var value)) {
				if (_valueComparer.Equals(item.Value, value)) {
					RemoveAt(index);
					return true;
				}
			}
			return false;
		}
	}

	public void RemoveAt(long index) {
		CheckLoaded();
		// We don't delete the instance, we mark is as unused. Use Shrink to intelligently remove unused records.
		using (ObjectStream.EnterAccessScope()) {
			ObjectStream.ReapItem(index);
			UpdateVersion();
		}
	}

	public override bool Contains(KeyValuePair<TKey, TValue> item) {
		Guard.ArgumentNotNull(item, nameof(item));
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			if (!TryFindValue(item.Key, out _, out var value))
				return false;
			return _valueComparer.Equals(value, item.Value);
		}
	}

	public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
		CheckLoaded();
		using (ObjectStream.EnterAccessScope())
			KeyValuePairs.ToArray().CopyTo(array, arrayIndex);
	}

	public void Shrink() {
		// delete all unused records from _kvpStore
		// deletes item right to left
		// possible optimization: a connected neighbourhood of unused records can be deleted 
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			var sortedList = new SortedList<long>(SortDirection.Descending);
			_recyclableIndexStore.Store.ForEach(sortedList.Add);
			foreach (var recycledIndex in sortedList) {
				ObjectStream.RemoveItem(recycledIndex);
			}
			_recyclableIndexStore.Store.Clear();
			UpdateVersion();
		}
	}

	public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			var version = Version;
			for (var i = 0; i < ObjectStream.Count; i++) {
				CheckVersion(version);
				if (ObjectStream.GetItemDescriptor(i).Traits.HasFlag(ClusteredStreamTraits.Reaped))
					continue;
				var kvp = ObjectStream.LoadItem(i);
				yield return kvp;
			}
		}
	}

	protected override IEnumerator<TKey> GetKeysEnumerator() {
		CheckLoaded();
		var version = Version;
		for (var i = 0; i < ObjectStream.Count; i++) {
			CheckVersion(version);
			if (ObjectStream.GetItemDescriptor(i).Traits.HasFlag(ClusteredStreamTraits.Reaped))
				continue;
			var key = ReadKey(i);
			yield return key;
		}
	}

	protected override IEnumerator<TValue> GetValuesEnumerator() {
		CheckLoaded();
		var version = Version;
		for (var i = 0; i < ObjectStream.Count; i++) {
			CheckVersion(version);
			if (ObjectStream.GetItemDescriptor(i).Traits.HasFlag(ClusteredStreamTraits.Reaped))
				continue;
			var key = ReadValue(i);
			yield return key;
		}
	}

	private void AddInternal(KeyValuePair<TKey, TValue> item) {
		long index;
		if (_recyclableIndexStore.Store.Any()) {
			index = _recyclableIndexStore.Store.Pop();
			ObjectStream.SaveItem(index, item, ObjectStreamOperationType.Update);
		} else {
			index = Count;
			ObjectStream.SaveItem(index, item, ObjectStreamOperationType.Add);
		}
		UpdateVersion();
	}

	private void UpdateInternal(long index, KeyValuePair<TKey, TValue> item) {
		// Updating value only, records (and checksum) don't change  when updating
		ObjectStream.SaveItem(index, item, ObjectStreamOperationType.Update);
		UpdateVersion();
	}

	private void CheckLoaded() {
		if (RequiresLoad)
			throw new InvalidOperationException($"{nameof(StreamMappedDictionary<TKey, TValue>)} has not been loaded");
	}

}