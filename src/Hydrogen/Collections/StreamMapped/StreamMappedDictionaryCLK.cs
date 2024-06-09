// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hydrogen.ObjectSpaces;

namespace Hydrogen;

/// <summary>
/// A dictionary whose contents are mapped onto a stream using an <see cref="ObjectStream"/> which in turn uses a <see cref="ClusteredStreams"/>.
/// This implementation is specialized for "constant-length keys" (i.e CLK) and stores only the <see cref="TValue "/> in the object objectStream, not KeyValuePair's
/// like <see cref="StreamMappedDictionary{TKey,TValue}"/>. The keys are stored in a separate index in a reserved stream within the <see cref="ClusteredStreams"/>.
/// Like <see cref="StreamMappedDictionary{TKey,TValue}"/>, deleted items are not removed from the underlying stream, but rather marked as reaped
/// and re-used in subsequent <see cref="Add(TKey,TValue)"/> operations.
/// </summary>
/// <typeparam name="TKey">The type of key stored in the dictionary</typeparam>
/// <typeparam name="TValue">The type of value stored in the dictionary</typeparam>
/// <remarks>Whilst this implementation can still be used with variable-length keys (via a <see cref="PaddedSerializer{TItem}"/>
/// it is strongly advised AGAINST this as key comparison does not consider the padding and this can lead
/// to potential key collisions. This class should be use with keys that are logically constant in length.</remarks>
public class StreamMappedDictionaryCLK<TKey, TValue> : DictionaryBase<TKey, TValue>, IStreamMappedDictionary<TKey, TValue> {
	public event EventHandlerEx<object> Loading { add => ObjectStream.Loading += value; remove => ObjectStream.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => ObjectStream.Loaded += value; remove => ObjectStream.Loaded -= value; }

	private readonly IEqualityComparer<TValue> _valueComparer;
	private readonly UniqueKeyStorageAttachment<TKey> _uniqueKeyStore;
	private readonly RecyclableIndexIndex _recyclableIndexIndex;

	internal StreamMappedDictionaryCLK(ObjectStream objectStream, string recyclableIndexName, string keyStoreName, IEqualityComparer<TValue> valueComparer = null, bool autoLoad = false) {
		Guard.ArgumentNotNull(objectStream, nameof(objectStream));
		ObjectStream = (ObjectStream<TValue>)objectStream;
		_recyclableIndexIndex = (RecyclableIndexIndex)objectStream.Streams.Attachments[recyclableIndexName];
		_uniqueKeyStore = (UniqueKeyStorageAttachment<TKey>)objectStream.Streams.Attachments[keyStoreName];
		_valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
		
		if (autoLoad && RequiresLoad)
			Load();
	}
	
	ObjectStream IStreamMappedCollection.ObjectStream => ObjectStream;

	public ObjectStream<TValue> ObjectStream { get; }

	protected IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs => GetEnumerator().AsEnumerable();

	public bool RequiresLoad => ObjectStream.RequiresLoad;

	public override long Count {
		get {
			CheckLoaded();
			using (ObjectStream.EnterAccessScope())
				return ObjectStream.Count - _recyclableIndexIndex.Store.Count;
		}	
	}

	public override bool IsReadOnly => false;

	public bool OwnsContainer { get; set; }

	public void Load() => ObjectStream.Load();

	public Task LoadAsync() => Task.Run(Load);

	public virtual void Dispose() {
		if (OwnsContainer)
			ObjectStream.Dispose();
	}
	
	public TKey ReadKey(long index) {
		using (ObjectStream.EnterAccessScope()) {
			var traits = ObjectStream.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");

			return _uniqueKeyStore.Read(index);
		}
	}

	public byte[] ReadKeyBytes(long index) {
		using (ObjectStream.EnterAccessScope()) {
			var traits = ObjectStream.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");

			return _uniqueKeyStore.ReadBytes(index);
		}
	}

	public TValue ReadValue(long index) {
		using (ObjectStream.EnterAccessScope()) {
			var traits = ObjectStream.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");
			
			if (traits.HasFlag(ClusteredStreamTraits.Null))
				return default;
			
			return ObjectStream.LoadItem(index);
		}
	}

	public byte[] ReadValueBytes(long index) {
		using (ObjectStream.EnterAccessScope()) {
			var traits = ObjectStream.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");
			
			if (traits.HasFlag(ClusteredStreamTraits.Null))
				return null;

			return ObjectStream.GetItemBytes(index);
		}
	}

	public override void Add(TKey key, TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			if (TryFindKey(key, out _))
				throw new KeyNotFoundException($"An item with key '{key}' was already added");
			AddInternal(key, value);
		}
	}

	public override void Update(TKey key, TValue value) {
		using (ObjectStream.EnterAccessScope()) {
			if (!TryFindKey(key, out var index))
				throw new KeyNotFoundException($"The key '{key}' was not found");
			UpdateInternal(index, key, value);
		}
	}

	protected override void AddOrUpdate(TKey key, TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			if (TryFindKey(key, out var index)) {
				UpdateInternal(index, key, value);
			} else {
				AddInternal(key, value);
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
		using (ObjectStream.EnterAccessScope())
			return _uniqueKeyStore.ContainsKey(key);
	}

	public override void Clear() {
		// Load not required
		using (ObjectStream.EnterAccessScope()) {
			_uniqueKeyStore.Clear();
			_uniqueKeyStore.Detach();
			ObjectStream.Clear();
			_uniqueKeyStore.Attach();
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
		Guard.ArgumentNotNull(key, nameof(key));
		using (ObjectStream.EnterAccessScope()) {
			if (_uniqueKeyStore.TryGetValue(key, out index)) {
				return true;
			}
			index = -1;
			return false;
		}
	}

	public bool TryFindValue(TKey key, out long index, out TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		using (ObjectStream.EnterAccessScope()) {
			if (_uniqueKeyStore.TryGetValue(key, out index)) {
				value = ReadValue(index);
				return true;
			}
			index = -1;
			value = default;
			return false;
		}
	}

	public override void Add(KeyValuePair<TKey, TValue> item) {
		Guard.ArgumentNotNull(item, nameof(item)); // Key not null checked in TrueFindKey
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			if (TryFindKey(item.Key, out _))
				throw new KeyNotFoundException($"An item with key '{item.Key}' was already added");
			AddInternal(item.Key, item.Value);
		}
	}

	public override bool Remove(KeyValuePair<TKey, TValue> item) {
		Guard.ArgumentNotNull(item, nameof(item)); // Key not null checked in TryFindValue
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
			_uniqueKeyStore.Reap(index);
			ObjectStream.ReapItem(index);
			UpdateVersion();
		}
	}

	public override bool Contains(KeyValuePair<TKey, TValue> item) {
		Guard.ArgumentNotNull(item, nameof(item)); // Key not null checked in TryFindValue
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			if (!TryFindValue(item.Key, out _, out var value))
				return false;
			return _valueComparer.Equals(item.Value, value);
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
			_recyclableIndexIndex.Store.ForEach(sortedList.Add);
			foreach (var recyclableIndex in sortedList) {
				ObjectStream.RemoveItem(recyclableIndex);
			}
			_recyclableIndexIndex.Store.Clear();
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
				var kvp = new KeyValuePair<TKey, TValue>(ReadKey(i), ObjectStream.LoadItem(i));
				yield return kvp;
			}
		}
	}

	protected override IEnumerator<TKey> GetKeysEnumerator() {
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			var version = Version;
			for (var i = 0; i < ObjectStream.Count; i++) {
				CheckVersion(version);
				if (ObjectStream.GetItemDescriptor(i).Traits.HasFlag(ClusteredStreamTraits.Reaped))
					continue;
				var key = ReadKey(i);
				yield return key;
			}
		}
	}

	protected override IEnumerator<TValue> GetValuesEnumerator() {
		CheckLoaded();
		using (ObjectStream.EnterAccessScope()) {
			var version = Version;
			for (var i = 0; i < ObjectStream.Count; i++) {
				CheckVersion(version);
				if (ObjectStream.GetItemDescriptor(i).Traits.HasFlag(ClusteredStreamTraits.Reaped))
					continue;
				var key = ReadValue(i);
				yield return key;
			}
		}
	}

	private void AddInternal(TKey key, TValue value) {
		long index;
		if (_recyclableIndexIndex.Store.Any()) {
			index = _recyclableIndexIndex.Store.Pop();
			_uniqueKeyStore.Update(index, key);
			ObjectStream.SaveItem(index, value, ObjectStreamOperationType.Update);
		} else {
			index = Count;
			_uniqueKeyStore.Add(index, key);
			ObjectStream.SaveItem(index, value, ObjectStreamOperationType.Add);
		}
		UpdateVersion();
	}

	private void UpdateInternal(long index, TKey key, TValue value) {
		// Updating value only, records (and checksum) don't change  when updating
		_uniqueKeyStore.Update(index, key);
		ObjectStream.SaveItem(index, value, ObjectStreamOperationType.Update);
		UpdateVersion();
	}

	private void CheckLoaded() {
		if (RequiresLoad)
			throw new InvalidOperationException($"{nameof(StreamMappedDictionary<TKey, TValue>)} has not been loaded");
	}

}
