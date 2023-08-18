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
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// Similar to a <see cref="StreamMappedDictionary{TKey,TValue}"/> except keys are statically sized and serialized inside the <see cref="ClusteredStreamDescriptor"/> rather than within a <see cref="KeyValuePair{TKey, TValue}"/> item).
/// This implementation of <see cref="IStreamMappedDictionary{TKey,TValue}"/> permits faster scanning of keys and is thus used internally by <see cref="StreamMappedHashSet{TItem}"/> and <see cref="MerklizedHashSet{TValue}"/> for
/// their implementations.
/// </summary>
/// <typeparam name="TKey">The type of key stored in the dictionary</typeparam>
/// <typeparam name="TValue">The type of value stored in the dictionary</typeparam>
/// <remarks>When deleting an item the underlying <see cref="ClusteredStreamDescriptor"/> is marked nullified but retained and re-used in later calls to <see cref="Add(TKey,TValue)"/>.</remarks>
/// <remarks>This implementation stores the items key in the underlying <see cref="ClusteredStreamDescriptor"/> whereas the <see cref="StreamMappedDictionary{TKey,TValue}"/> class stores it in the <see cref="KeyValuePair{TKey, TValue}"/>.</remarks>
// TODO: there are some memory-blowout lookups in this class that need to be refactored out (should be safe for non-huge dictionaries).
public class StreamMappedDictionarySK<TKey, TValue> : DictionaryBase<TKey, TValue>, IStreamMappedDictionary<TKey, TValue> {
	public event EventHandlerEx<object> Loading;
	public event EventHandlerEx<object> Loaded;

	public readonly byte[] UnusedKeyBytes;
	private readonly IItemChecksummer<TKey> _keyChecksum;
	private readonly IItemSerializer<TKey> _keySerializer;
	private readonly IItemSerializer<TValue> _valueSerializer;
	private readonly IStreamMappedList<TValue> _valueStore;
	private readonly IEqualityComparer<TKey> _keyComparer;
	private readonly IEqualityComparer<TValue> _valueComparer;
	private readonly LookupEx<int, int> _checksumToIndexLookup;
	private readonly SortedList<int> _unusedDescriptors;
	private readonly bool _preAllocateOptimization;
	private bool _requiresLoad;

	public StreamMappedDictionarySK(Stream rootStream, int clusterSize, IItemSerializer<TKey> keyStaticSizedSerializer, IItemSerializer<TValue> valueSerializer = null, IItemChecksummer<TKey> keyChecksum = null,
									IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null, StreamContainerPolicy policy = StreamContainerPolicy.DictionaryDefault, long reservedRecords = 0,
									Endianness endianness = Endianness.LittleEndian, bool autoLoad = false)
		: this(
			new StreamMappedList<TValue>(
				rootStream,
				clusterSize,
				valueSerializer,
				valueComparer,
				policy | StreamContainerPolicy.TrackChecksums | StreamContainerPolicy.TrackKey,
				keyStaticSizedSerializer.StaticSize,
				reservedRecords,
				endianness
			),
			keyStaticSizedSerializer,
			valueSerializer,
			keyChecksum,
			keyComparer,
			valueComparer,
			endianness,
			autoLoad
		) {
	}

	public StreamMappedDictionarySK(IStreamMappedList<TValue> valueStore, IItemSerializer<TKey> keyStaticSizedSerializer, IItemSerializer<TValue> valueSerializer = null, IItemChecksummer<TKey> keyChecksummer = null,
	                                IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null, Endianness endianness = Endianness.LittleEndian, bool autoLoad = false) {
		Guard.ArgumentNotNull(keyStaticSizedSerializer, nameof(keyStaticSizedSerializer));
		Guard.Argument(keyStaticSizedSerializer.IsStaticSize, nameof(keyStaticSizedSerializer), "Keys must be statically sized");
		Guard.Argument(valueStore.Streams.Policy.HasFlag(StreamContainerPolicy.TrackChecksums), nameof(valueStore), $"Checksum tracking must be enabled in {nameof(StreamMappedDictionarySK<TKey, TValue>)} implementations.");
		Guard.Argument(valueStore.Streams.Policy.HasFlag(StreamContainerPolicy.TrackKey), nameof(valueStore), $"Key tracking must be enabled in {nameof(StreamMappedDictionarySK<TKey, TValue>)} implementations.");

		_keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
		_valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
		_valueStore = valueStore;
		_keySerializer = keyStaticSizedSerializer;
		_valueSerializer = valueSerializer ?? ItemSerializer<TValue>.Default;
		_keyChecksum = keyChecksummer ?? new ItemDigestor<TKey>(keyStaticSizedSerializer, endianness);
		_checksumToIndexLookup = new LookupEx<int, int>();
		_unusedDescriptors = new();
		_requiresLoad = true; //_valueStore.Streams.Records.Count > _valueStore.Streams.Header.ReservedStreams;
		UnusedKeyBytes = Tools.Array.Gen<byte>(_keySerializer.StaticSize, 0);
		_preAllocateOptimization = Streams.Policy.HasFlag(StreamContainerPolicy.FastAllocate);
		if (autoLoad)
			Load();
	}

	public StreamContainer Streams => _valueStore.Streams;

	protected IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs {
		get {
			for (var i = 0; i < _valueStore.Count; i++) {
				if (IsUnusedRecord(i))
					continue;
				var descriptor = _valueStore.Streams.GetStreamDescriptor(_valueStore.Streams.Header.ReservedStreams + i);
				yield return new KeyValuePair<TKey, TValue>(
					_keySerializer.Deserialize(descriptor.Key, Streams.Endianness),
					!descriptor.Traits.HasFlag(ClusteredStreamTraits.Null) ? _valueStore[i] : default
				);
			}
		}
	}

	public bool RequiresLoad {
		get => _valueStore.RequiresLoad || _requiresLoad;
		private set => _requiresLoad = value;
	}

	public override int Count {
		get {
			CheckLoaded();
			var valueStoreCountI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(_valueStore.Count);
			var unusedRecordsCountI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(_unusedDescriptors.Count);
			return valueStoreCountI - unusedRecordsCountI;
		}
	}

	public override bool IsReadOnly => false;

	public void Load() {
		NotifyLoading();
		if (_valueStore.RequiresLoad)
			_valueStore.Load();
		RefreshChecksumToIndexLookup();
		RequiresLoad = false;
		NotifyLoaded();
	}

	public Task LoadAsync() => Task.Run(Load);

	public TKey ReadKey(int index) {
		using (Streams.EnterAccessScope()) {
			if (Streams.IsNull(_valueStore.Streams.Header.ReservedStreams + index))
				throw new InvalidOperationException($"Stream record {index} is null");
	
			var record = Streams.GetStreamDescriptor(_valueStore.Streams.Header.ReservedStreams + index);
			return _keySerializer.Deserialize(record.Key, Streams.Endianness);
		}
	}

	public TValue ReadValue(int index) {
		using (Streams.EnterAccessScope()) {
			if (Streams.IsNull(_valueStore.Streams.Header.ReservedStreams + index))
				return default;

			return _valueStore.Read(index);
		}
	}

	public override void Add(TKey key, TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (Streams.EnterAccessScope()) {
			if (TryFindKey(key, out _))
				throw new KeyNotFoundException($"An item with key '{key}' was already added");
			AddInternal(key, value);
		}
	}

	public override void Update(TKey key, TValue value) {
		using (Streams.EnterAccessScope()) {
			if (!TryFindKey(key, out var index))
				throw new KeyNotFoundException($"The key '{key}' was not found");
			UpdateInternal(index, key, value);
		}
	}

	protected override void AddOrUpdate(TKey key, TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (Streams.EnterAccessScope()) {
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
		using (Streams.EnterAccessScope()) {
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
		using (Streams.EnterAccessScope()) {
			return
				_checksumToIndexLookup[_keyChecksum.CalculateChecksum(key)]
					.Where(index => !IsUnusedRecord(index))
					.Select(ReadKey)
					.Any(item => _keyComparer.Equals(item, key));
		}
	}

	public override void Clear() {
		// Load not required
		using (Streams.EnterAccessScope()) {
			_valueStore.Clear();
			_checksumToIndexLookup.Clear();
			_unusedDescriptors.Clear();
		}
	}

	public override bool TryGetValue(TKey key, out TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		CheckLoaded();
		using (Streams.EnterAccessScope()) {
			if (TryFindValue(key, out _, out value)) {
				return true;
			}
			value = default;
			return false;
		}
	}

	public bool TryFindKey(TKey key, out int index) {
		Guard.ArgumentNotNull(key, nameof(key));
		using (Streams.EnterAccessScope()) {
			foreach (var i in _checksumToIndexLookup[_keyChecksum.CalculateChecksum(key)]) {
				var candidateKey = ReadKey(i);
				if (_keyComparer.Equals(candidateKey, key)) {
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}
	}

	public bool TryFindValue(TKey key, out int index, out TValue value) {
		Guard.ArgumentNotNull(key, nameof(key));
		using (Streams.EnterAccessScope()) {
			foreach (var i in _checksumToIndexLookup[_keyChecksum.CalculateChecksum(key)]) {
				var candidateKey = ReadKey(i);
				if (_keyComparer.Equals(candidateKey, key)) {
					index = i;
					value = ReadValue(index);
					return true;
				}
			}
			index = -1;
			value = default;
			return false;
		}
	}

	public override void Add(KeyValuePair<TKey, TValue> item) {
		Guard.ArgumentNotNull(item, nameof(item)); // Key not null checked in TrueFindKey
		CheckLoaded();
		using (Streams.EnterAccessScope()) {
			if (TryFindKey(item.Key, out _))
				throw new KeyNotFoundException($"An item with key '{item.Key}' was already added");
			AddInternal(item.Key, item.Value);
		}
	}

	public override bool Remove(KeyValuePair<TKey, TValue> item) {
		Guard.ArgumentNotNull(item, nameof(item)); // Key not null checked in TryFindValue
		CheckLoaded();
		using (Streams.EnterAccessScope()) {
			if (TryFindValue(item.Key, out var index, out var value)) {
				if (_valueComparer.Equals(item.Value, value)) {
					RemoveAt(index);
					return true;
				}
			}
			return false;
		}
	}

	public void RemoveAt(int index) {
		// We don't delete the instance, we mark is as unused. Use Shrink to intelligently remove unused records.
		using var stream = _valueStore.EnterUpdateScope(index, default);

		// Mark record as unused
		Guard.Ensure(!stream.IsTomb, "Descriptor not in used state");
		_checksumToIndexLookup.Remove(stream.KeyChecksum, index);
		stream.Key = UnusedKeyBytes;
		stream.KeyChecksum = -1;
		stream.IsTomb = true;
		_unusedDescriptors.Add(index);
		// note: scope Dispose updates record
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
		// delete all unused records from _valueStore
		// deletes item right to left
		// possible optimization: a connected neighbourhood of unused records can be deleted 
		CheckLoaded();
		for (var i = _unusedDescriptors.Count - 1; i >= 0; i--) {
			var index = _unusedDescriptors[i];
			_valueStore.RemoveAt(index);
			_unusedDescriptors.RemoveAt(i);
		}
	}

	public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
		CheckLoaded();
		for (var i = 0; i < _valueStore.Count; i++) {
			if (_unusedDescriptors.Contains(i))
				continue;
			KeyValuePair<TKey, TValue> kvp;
			using (var stream = _valueStore.Streams.EnterLoadItemScope(Streams.Header.ReservedStreams + i, _valueSerializer, out var value, _preAllocateOptimization))
				kvp = new KeyValuePair<TKey, TValue>(_keySerializer.Deserialize(stream.Key, Streams.Endianness), value);
			yield return kvp;
		}
	}

	protected override IEnumerator<TKey> GetKeysEnumerator() {
		CheckLoaded();
		return Enumerable.Range(0, Tools.Collection.CheckNotImplemented64bitAddressingLength(_valueStore.Count))
			.Where(i => !_unusedDescriptors.Contains(i))
			.Select(ReadKey)
			.GetEnumerator();
	}

	protected override IEnumerator<TValue> GetValuesEnumerator() {
		CheckLoaded();
		return Enumerable.Range(0, Tools.Collection.CheckNotImplemented64bitAddressingLength(_valueStore.Count))
			.Where(i => !_unusedDescriptors.Contains(i))
			.Select(ReadValue)
			.GetEnumerator();
	}

	protected virtual void OnLoading() {
	}

	protected virtual void OnLoaded() {
	}

	private void AddInternal(TKey key, TValue value) {
		int index;
		ClusteredStream stream;
		if (_unusedDescriptors.Any()) {
			index = ConsumeUnusedRecord();
			stream = _valueStore.EnterUpdateScope(index, value);
		} else {
			stream = _valueStore.EnterAddScope(value);
			stream.IsTomb = true;
			index = Count - 1;
		}

		using (stream) {
			// Mark record as used and set key
			Guard.Ensure(stream.IsTomb, $"Stream {index} is not available for use");
			stream.Key = _keySerializer.Serialize(key, Streams.Endianness);
			stream.KeyChecksum = _keyChecksum.CalculateChecksum(key);
			stream.IsTomb = false;
			_checksumToIndexLookup.Add(stream.KeyChecksum, index);
			// note: stream disposal updates descriptor to stream
		}
	}

	private void UpdateInternal(int index, TKey key, TValue value) {
		using var stream = _valueStore.EnterUpdateScope(index, value);
		stream.IsTomb = false;
		stream.Key = _keySerializer.Serialize(key, Streams.Endianness);
		stream.KeyChecksum = _keyChecksum.CalculateChecksum(key);
		// note: scope Dispose updates record
	}

	private bool IsUnusedRecord(int index) => _unusedDescriptors.Contains(index);

	private int ConsumeUnusedRecord() {
		var index = _unusedDescriptors[0];
		_unusedDescriptors.RemoveAt(0);
		return index;
	}

	private void RefreshChecksumToIndexLookup() {
		_checksumToIndexLookup.Clear();
		_unusedDescriptors.Clear();
		for (var i = 0; i < _valueStore.Count; i++) {
			var reservedStreamsI = Tools.Collection.CheckNotImplemented64bitAddressingLength(_valueStore.Streams.Header.ReservedStreams);
			var descriptor = _valueStore.Streams.GetStreamDescriptor(reservedStreamsI + i);
			if (descriptor.Traits.HasFlag(ClusteredStreamTraits.Tomb))
				_checksumToIndexLookup.Add(descriptor.KeyChecksum, i);
			else
				_unusedDescriptors.Add(i);
		}
	}

	private void CheckLoaded() {
		if (RequiresLoad)
			throw new InvalidOperationException($"{nameof(StreamMappedDictionary<TKey, TValue>)} has not been loaded");
	}

	private void NotifyLoading() {
		OnLoading();
		Loading?.Invoke(this);
	}

	private void NotifyLoaded() {
		OnLoaded();
		Loaded?.Invoke(this);
	}

}
