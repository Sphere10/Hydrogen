// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
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
/// A dictionary whose contents are mapped onto a stream using an <see cref="ObjectContainer"/> which in turn uses a <see cref="StreamContainer"/>.
/// This implementation is specialized for "constant-length keys" (i.e CLK) and stores only the <see cref="TValue "/> in the object container, not KeyValuePair's
/// like <see cref="StreamMappedDictionary{TKey,TValue}"/>. The keys are stored in a separate index in a reserved stream within the <see cref="StreamContainer"/>.
/// Like <see cref="StreamMappedDictionary{TKey,TValue}"/>, deleted items are not removed from the underlying stream, but rather marked as reaped
/// and re-used in subsequent <see cref="Add(TKey,TValue)"/> operations.
/// </summary>
/// <typeparam name="TKey">The type of key stored in the dictionary</typeparam>
/// <typeparam name="TValue">The type of value stored in the dictionary</typeparam>
/// <remarks>Whilst this implementation can still be used with variable-length keys (via a <see cref="PaddedSerializer{TItem}"/>
/// it is strongly advised AGAINST this as key comparison does not consider the padding and this can lead
/// to potential key collisions. This class should be use with keys that are logically constant in length.</remarks>
public class StreamMappedDictionaryCLK<TKey, TValue> : DictionaryBase<TKey, TValue>, IStreamMappedDictionary<TKey, TValue> {
	public event EventHandlerEx<object> Loading { add => ObjectContainer.Loading += value; remove => ObjectContainer.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => ObjectContainer.Loaded += value; remove => ObjectContainer.Loaded -= value; }

	private readonly IEqualityComparer<TValue> _valueComparer;
	private readonly UniqueKeyStore<TKey> _keyStore;
	private readonly RecyclableIndexIndex _recyclableIndexIndex;

	internal StreamMappedDictionaryCLK(
		ObjectContainer objectContainer,
		RecyclableIndexIndex recyclableIndexIndex,
		UniqueKeyStore<TKey> keyStore,
		IEqualityComparer<TValue> valueComparer = null,
		bool autoLoad = false
	) {
		Guard.ArgumentNotNull(objectContainer, nameof(objectContainer));
		Guard.ArgumentNotNull(recyclableIndexIndex, nameof(recyclableIndexIndex));
		Guard.ArgumentNotNull(keyStore, nameof(keyStore));
		ObjectContainer = (ObjectContainer<TValue>)objectContainer;
		_recyclableIndexIndex = recyclableIndexIndex;
		_keyStore = keyStore;
		_valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
		
		// This ensures _keyStore is attached/detached as required by container operations
		ObjectContainer.RegisterAttachment(_keyStore);
		
		if (autoLoad && RequiresLoad)
			Load();
	}
	
	ObjectContainer IStreamMappedDictionary<TKey, TValue>.ObjectContainer => ObjectContainer;

	public ObjectContainer<TValue> ObjectContainer { get; }

	protected IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs => GetEnumerator().AsEnumerable();

	public bool RequiresLoad => ObjectContainer.RequiresLoad;

	public override long Count {
		get {
			CheckLoaded();
			return ObjectContainer.Count - _recyclableIndexIndex.Stack.Count;
		}
	}

	public override bool IsReadOnly => false;

	public bool OwnsContainer { get; set; }

	public void Load() => ObjectContainer.Load();

	public Task LoadAsync() => Task.Run(Load);

	public virtual void Dispose() {
		if (OwnsContainer)
			ObjectContainer.Dispose();
	}
	
	public TKey ReadKey(long index) {
		using (ObjectContainer.EnterAccessScope()) {
			var traits = ObjectContainer.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");

			return _keyStore.Read(index);
		}
	}

	public byte[] ReadKeyBytes(long index) {
		using (ObjectContainer.EnterAccessScope()) {
			var traits = ObjectContainer.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");

			return _keyStore.ReadBytes(index);
		}
	}

	public TValue ReadValue(long index) {
		using (ObjectContainer.EnterAccessScope()) {
			var traits = ObjectContainer.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");
			
			if (traits.HasFlag(ClusteredStreamTraits.Null))
				return default;
			
			return ObjectContainer.LoadItem(index);
		}
	}

	public byte[] ReadValueBytes(long index) {
		using (ObjectContainer.EnterAccessScope()) {
			var traits = ObjectContainer.GetItemDescriptor(index).Traits;
			if (traits.HasFlag(ClusteredStreamTraits.Reaped))
				throw new InvalidOperationException($"Object {index} has been reaped");
			
			if (traits.HasFlag(ClusteredStreamTraits.Null))
				return null;

			return ObjectContainer.GetItemBytes(index);
		}
	}

	public override void Add(TKey key, TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (ObjectContainer.EnterAccessScope()) {
			if (TryFindKey(key, out _))
				throw new KeyNotFoundException($"An item with key '{key}' was already added");
			AddInternal(key, value);
		}
	}

	public override void Update(TKey key, TValue value) {
		using (ObjectContainer.EnterAccessScope()) {
			if (!TryFindKey(key, out var index))
				throw new KeyNotFoundException($"The key '{key}' was not found");
			UpdateInternal(index, key, value);
		}
	}

	protected override void AddOrUpdate(TKey key, TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (ObjectContainer.EnterAccessScope()) {
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
		using (ObjectContainer.EnterAccessScope()) {
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
		using (ObjectContainer.EnterAccessScope())
			return _keyStore.Dictionary.ContainsKey(key);
	}

	public override void Clear() {
		// Load not required
		using (ObjectContainer.EnterAccessScope()) {
			ObjectContainer.Clear();
			UpdateVersion();
		}
	}

	public override bool TryGetValue(TKey key, out TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (ObjectContainer.EnterAccessScope()) {
			if (TryFindValue(key, out _, out value)) {
				return true;
			}
			value = default;
			return false;
		}
	}

	public bool TryFindKey(TKey key, out long index) {
		Guard.ArgumentNotNull(key, nameof(key));
		using (ObjectContainer.EnterAccessScope()) {
			if (_keyStore.Dictionary.TryGetValue(key, out index)) {
				return true;
			}
			index = -1;
			return false;
		}
	}

	public bool TryFindValue(TKey key, out long index, out TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		using (ObjectContainer.EnterAccessScope()) {
			if (_keyStore.Dictionary.TryGetValue(key, out index)) {
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
		using (ObjectContainer.EnterAccessScope()) {
			if (TryFindKey(item.Key, out _))
				throw new KeyNotFoundException($"An item with key '{item.Key}' was already added");
			AddInternal(item.Key, item.Value);
		}
	}

	public override bool Remove(KeyValuePair<TKey, TValue> item) {
		Guard.ArgumentNotNull(item, nameof(item)); // Key not null checked in TryFindValue
		CheckLoaded();
		using (ObjectContainer.EnterAccessScope()) {
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
		using (ObjectContainer.EnterAccessScope()) {
			_keyStore.Reap(index);
			ObjectContainer.ReapItem(index);
			UpdateVersion();
		}
	}

	public override bool Contains(KeyValuePair<TKey, TValue> item) {
		Guard.ArgumentNotNull(item, nameof(item)); // Key not null checked in TryFindValue
		CheckLoaded();
		if (!TryFindValue(item.Key, out _, out var value))
			return false;
		return _valueComparer.Equals(item.Value, value);
	}

	public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
		CheckLoaded();
		KeyValuePairs.ToArray().CopyTo(array, arrayIndex);
	}

	public void Shrink() {
		// delete all unused records from _kvpStore
		// deletes item right to left
		// possible optimization: a connected neighbourhood of unused records can be deleted 
		CheckLoaded();
		var sortedList = new SortedList<long>(SortDirection.Descending);
		_recyclableIndexIndex.Stack.ForEach(sortedList.Add);
		foreach (var freeIndex in sortedList) {
			ObjectContainer.RemoveItem(freeIndex);
		}
		_recyclableIndexIndex.Stack.Clear();
		UpdateVersion();
	}

	public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
		CheckLoaded();
		var version = Version;
		for (var i = 0; i < ObjectContainer.Count; i++) {
			CheckVersion(version);
			if (ObjectContainer.GetItemDescriptor(i).Traits.HasFlag(ClusteredStreamTraits.Reaped))
				continue;
			var kvp = new KeyValuePair<TKey, TValue>(ReadKey(i), ObjectContainer.LoadItem(i));
			yield return kvp;
		}
	}

	protected override IEnumerator<TKey> GetKeysEnumerator() {
		CheckLoaded();
		var version = Version;
		for (var i = 0; i < ObjectContainer.Count; i++) {
			CheckVersion(version);
			if (ObjectContainer.GetItemDescriptor(i).Traits.HasFlag(ClusteredStreamTraits.Reaped))
				continue;
			var key = ReadKey(i);
			yield return key;
		}
	}

	protected override IEnumerator<TValue> GetValuesEnumerator() {
		CheckLoaded();
		var version = Version;
		for (var i = 0; i < ObjectContainer.Count; i++) {
			CheckVersion(version);
			if (ObjectContainer.GetItemDescriptor(i).Traits.HasFlag(ClusteredStreamTraits.Reaped))
				continue;
			var key = ReadValue(i);
			yield return key;
		}
	}

	private void AddInternal(TKey key, TValue value) {
		long index;
		if (_recyclableIndexIndex.Stack.Any()) {
			index = _recyclableIndexIndex.Stack.Pop();
			_keyStore.Update(index, key);
			ObjectContainer.SaveItem(index, value, ObjectContainerOperationType.Update);
		} else {
			index = Count;
			_keyStore.Add(index, key);
			ObjectContainer.SaveItem(index, value, ObjectContainerOperationType.Add);
		}
		UpdateVersion();
	}

	private void UpdateInternal(long index, TKey key, TValue value) {
		// Updating value only, records (and checksum) don't change  when updating
		_keyStore.Update(index, key);
		ObjectContainer.SaveItem(index, value, ObjectContainerOperationType.Update);
		UpdateVersion();
	}

	private void CheckLoaded() {
		if (RequiresLoad)
			throw new InvalidOperationException($"{nameof(StreamMappedDictionary<TKey, TValue>)} has not been loaded");
	}

}
