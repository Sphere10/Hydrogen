using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

	/// <summary>
	/// Similar to a <see cref="ClusteredDictionary{TKey,TValue}"/> except keys are statically sized and serialized inside the <see cref="ClusteredStorageRecord"/> rather than within a <see cref="KeyValuePair{TKey, TValue}"/> item).
	/// This implementation of <see cref="IClusteredDictionary{TKey,TValue}"/> permits faster scanning of keys and is thus used internally by <see cref="ClusteredHashSet{TValue}"/> and <see cref="MerklizedHashSet{TValue}"/> for
	/// their implementations.
	/// </summary>
	/// <typeparam name="TKey">The type of key stored in the dictionary</typeparam>
	/// <typeparam name="TValue">The type of value stored in the dictionary</typeparam>
	/// <remarks>When deleting an item the underlying <see cref="ClusteredStorageRecord"/> is marked nullified but retained and re-used in later calls to <see cref="Add(TKey,TValue)"/>.</remarks>
	/// <remarks>This implementation stores the items key in the underlying <see cref="ClusteredStorageRecord"/> whereas the <see cref="ClusteredDictionary{TKey,TValue}"/> class stores it in the <see cref="KeyValuePair{TKey, TValue}"/>.</remarks>
	// TODO: there are some memory-blowout lookups in this class that need to be refactored out (should be safe for non-huge dictionaries).
	public class ClusteredDictionarySK<TKey, TValue> : DictionaryBase<TKey, TValue>, IClusteredDictionary<TKey, TValue>, ILoadable {
		public event EventHandlerEx<object> Loading;
		public event EventHandlerEx<object> Loaded;

		public readonly byte[] UnusedKeyBytes;
		private readonly IItemChecksum<TKey> _keyChecksum;
		private readonly IItemSerializer<TKey> _keySerializer;
		private readonly IItemSerializer<TValue> _valueSerializer;
		private readonly Endianness _endianness;
		private readonly IClusteredList<byte[]> _valueStore;
		private readonly IEqualityComparer<TKey> _keyComparer;
		private readonly LookupEx<int, int> _checksumToIndexLookup;
		private readonly SortedList<int> _unusedRecords;

		public ClusteredDictionarySK(Stream rootStream, int clusterSize, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, Endianness endianness = Endianness.LittleEndian)
			: this(
				new ClusteredList<byte[]>(
					rootStream,
					clusterSize,
					new ByteArraySerializer(),
					new ByteArrayEqualityComparer(),
					policy,
					keySerializer.StaticSize,
					endianness
				),
				keySerializer,
				valueSerializer,
				keyChecksum,
				keyComparer,
				endianness
			) {
			Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in {nameof(ClusteredDictionarySK<TKey, TValue>)} implementations.");
			Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackKey), nameof(policy), $"Checksum tracking must be enabled in {nameof(ClusteredDictionarySK<TKey, TValue>)} implementations.");
		}

		public ClusteredDictionarySK(IClusteredList<byte[]> valueStore, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, Endianness endianess = Endianness.LittleEndian) {
			Guard.ArgumentNotNull(keySerializer, nameof(keySerializer));
			Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
			Guard.Argument(keySerializer.IsStaticSize, nameof(keySerializer),"Keys must be statically sized");
			_keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
			_valueStore = valueStore;
			_keySerializer = keySerializer;
			_valueSerializer = valueSerializer;
			_keyChecksum = keyChecksum ?? new ActionChecksum<TKey>(DefaultCalculateKeyChecksum);
			_endianness = endianess;
			_checksumToIndexLookup = new LookupEx<int, int>();
			_unusedRecords = new();
			RequiresLoad = _valueStore.Storage.Records.Count > 0;
			UnusedKeyBytes = Tools.Array.Gen<byte>(keySerializer.StaticSize, 0);
		}

		public IClusteredStorage Storage => _valueStore.Storage;

		protected IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs {
			get {
				for (var i = 0; i < _valueStore.Storage.Records.Count; i++) {
					if (IsUnusedRecord(i))
						continue;
					var (key, value) = (_valueStore.Storage.GetRecord(i), _valueStore[i]);
					yield return new KeyValuePair<TKey, TValue>(
						_keySerializer.Deserialize(key.Key, _endianness),
						ConvertValueFromBytes(value)
					);
				}
			}
		}

		public bool RequiresLoad { get; private set; }

		public override int Count {
			get {
				CheckLoaded();
				return _valueStore.Count - _unusedRecords.Count;
			}
		}

		public override bool IsReadOnly => false;

		public void Load() {
			NotifyLoading();
			RefreshChecksumToIndexLookup();
			RequiresLoad = false;
			NotifyLoaded();
		}

		public TKey ReadKey(int index) {
			if (Storage.IsNull(index))
				throw new InvalidOperationException($"Stream record {index} is null");
			var record = Storage.GetRecord(index);
			return _keySerializer.Deserialize(record.Key, _endianness);
		}

		public byte[] ReadValue(int index) {
			if (Storage.IsNull(index))
				throw new InvalidOperationException($"Stream record {index} is null");
			return _valueStore.Read(index);
		}

		public override void Add(TKey key, TValue value) {
			Guard.ArgumentNotNull(key, nameof(key));
			CheckLoaded();
			var newBytes = ConvertValueToBytes(value);
			if (TryFindKey(key, out var index)) {
				UpdateInternal(index, key, newBytes);
			} else {
				AddInternal(key, newBytes);
			}
		}

		public override bool Remove(TKey key) {
			Guard.ArgumentNotNull(key, nameof(key));
			CheckLoaded();
			if (TryFindKey(key, out var index)) {
				RemoveInternal(key, index);
				return true;
			}
			return false;
		}

		public override bool ContainsKey(TKey key) {
			Guard.ArgumentNotNull(key, nameof(key));
			CheckLoaded();
			return
				_checksumToIndexLookup[_keyChecksum.Calculate(key)]
					.Where(index => !IsUnusedRecord(index))
					.Select(ReadKey)
					.Any(item => _keyComparer.Equals(item, key));
		}

		public override void Clear() {
			// Load not required
			_valueStore.Clear();
			_checksumToIndexLookup.Clear();
			_unusedRecords.Clear();
		}

		public override bool TryGetValue(TKey key, out TValue value) {
			Guard.ArgumentNotNull(key, nameof(key));
			CheckLoaded();
			if (TryFindValue(key, out _, out var valueBytes)) {
				value = ConvertValueFromBytes(valueBytes);
				return true;
			}
			value = default;
			return false;
		}

		public override void Add(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			Guard.ArgumentNotNull(item, nameof(item));
			CheckLoaded();
			var newBytes = ConvertValueToBytes(item.Value);
			if (TryFindKey(item.Key, out var index)) {
				_valueStore.Update(index, newBytes);
			} else {
				AddInternal(item.Key, newBytes);
			}
		}

		public override bool Remove(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			CheckLoaded();
			if (TryFindValue(item.Key, out var index, out var valueBytes)) {
				var serializedValue = ConvertValueToBytes(item.Value);
				if (ByteArrayEqualityComparer.Instance.Equals(serializedValue, valueBytes)) {
					RemoveInternal(item.Key, index);
					return true;
				}
			}
			return false;
		}

		public override bool Contains(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			CheckLoaded();
			if (!TryFindValue(item.Key, out _, out var valueBytes))
				return false;
			var itemValueBytes = ConvertValueToBytes(item.Value);
			return ByteArrayEqualityComparer.Instance.Equals(valueBytes, itemValueBytes);
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
			for (var i = _unusedRecords.Count - 1; i >= 0; i--) {
				var index = _unusedRecords[i];
				_valueStore.RemoveAt(i);
				_unusedRecords.RemoveAt(i);
			}
		}

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			CheckLoaded();
			for (var i = 0; i < _valueStore.Count; i++) {
				if (_unusedRecords.Contains(i))
					continue;
				KeyValuePair<TKey, TValue> kvp;
				using (var scope = _valueStore.Storage.EnterLoadItemScope(i, _valueSerializer, out var value))
					kvp = new KeyValuePair<TKey, TValue>(_keySerializer.Deserialize(scope.Record.Key, _endianness), value);
				yield return kvp;
			}
		}

		protected override IEnumerator<TKey> GetKeysEnumerator() {
			CheckLoaded();
			return Enumerable.Range(0, _valueStore.Count)
							 .Where(i => !_unusedRecords.Contains(i))
							 .Select(ReadKey)
							 .GetEnumerator();
		}

		protected override IEnumerator<TValue> GetValuesEnumerator() {
			CheckLoaded();
			return Enumerable.Range(0, _valueStore.Count)
							 .Where(i => !_unusedRecords.Contains(i))
							 .Select(ReadValue)
							 .Select(ConvertValueFromBytes)
							 .GetEnumerator();
		}

		protected virtual void OnLoading() {
		}

		protected virtual void OnLoaded() {
		}

		private bool TryFindKey(TKey key, out int index) {
			Debug.Assert(key != null);
			foreach (var i in _checksumToIndexLookup[_keyChecksum.Calculate(key)]) {
				var candidateKey = ReadKey(i);
				if (_keyComparer.Equals(candidateKey, key)) {
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}

		private bool TryFindValue(TKey key, out int index, out byte[] valueBytes) {
			Debug.Assert(key != null);
			foreach (var i in _checksumToIndexLookup[_keyChecksum.Calculate(key)]) {
				var candidateKey = ReadKey(i);
				if (_keyComparer.Equals(candidateKey, key)) {
					index = i;
					valueBytes = ReadValue(index);
					return true;
				}
			}
			index = -1;
			valueBytes = default;
			return false;
		}

		private void AddInternal(TKey key, byte[] itemBytes) {
			int index;
			ClusteredStorageScope scope;
			if (_unusedRecords.Any()) {
				index = ConsumeUnusedRecord();
				scope = _valueStore.EnterUpdateScope(index, itemBytes);
			} else {
				scope = _valueStore.EnterAddScope(itemBytes);
				index = Count - 1;
			}


			using (scope) {
				// Mark record as used and set key
				Guard.Ensure(!scope.Record.Traits.HasFlag(ClusteredStorageRecordTraits.IsUsed), "Record not in unused state");
				scope.Record.Key = _keySerializer.Serialize(key, _endianness);
				scope.Record.KeyChecksum = _keyChecksum.Calculate(key);
				scope.Record.Traits = scope.Record.Traits.CopyAndSetFlags(ClusteredStorageRecordTraits.IsUsed, true);
				_checksumToIndexLookup.Add(scope.Record.KeyChecksum, index);
				// note: scope Dispose ends up updating the record
			}

		}

		private void UpdateInternal(int index, TKey key, byte[] itemBytes) {
			// Updating value only, records (and checksum) don't change  when updating
			using var scope = _valueStore.EnterUpdateScope(index, itemBytes);
			scope.Record.Traits = scope.Record.Traits.CopyAndSetFlags(ClusteredStorageRecordTraits.IsUsed, true);
			scope.Record.Key = _keySerializer.Serialize(key, _endianness);
			scope.Record.KeyChecksum = _keyChecksum.Calculate(key);
		}

		private void RemoveInternal(TKey key, int index) {
			// We don't delete the instance, we mark is as unused. Use Shrink to intelligently remove unused records.
			using var scope = _valueStore.EnterUpdateScope(index, null);

			// Mark record as unused
			Guard.Ensure(scope.Record.Traits.HasFlag(ClusteredStorageRecordTraits.IsUsed), "Record not in used state");
			_checksumToIndexLookup.Remove(scope.Record.KeyChecksum, index);
			scope.Record.Key = UnusedKeyBytes;
			scope.Record.KeyChecksum = -1;
			scope.Record.Traits = scope.Record.Traits.CopyAndSetFlags(ClusteredStorageRecordTraits.IsUsed, false);
			_unusedRecords.Add(index);

			// note: scope Dispose ends up updating the record
		}

		private bool IsUnusedRecord(int index) => _unusedRecords.Contains(index);

		private int ConsumeUnusedRecord() {
			var index = _unusedRecords[0];
			_unusedRecords.RemoveAt(0);
			return index;
		}

		private byte[] ConvertValueToBytes(TValue value)
			=> value != null ? _valueSerializer.Serialize(value, _endianness) : null;

		private TValue ConvertValueFromBytes(byte[] valueBytes)
			=> valueBytes != null ? _valueSerializer.Deserialize(valueBytes, _endianness) : default;

		private int DefaultCalculateKeyChecksum(TKey key)
			=> _keyComparer.GetHashCode(key);

		private void RefreshChecksumToIndexLookup() {
			_checksumToIndexLookup.Clear();
			_unusedRecords.Clear();
			for (var i = 0; i < _valueStore.Storage.Records.Count; i++) {
				var record = _valueStore.Storage.Records[i];
				if (record.Traits.HasFlag(ClusteredStorageRecordTraits.IsUsed))
					_checksumToIndexLookup.Add(record.KeyChecksum, i);
				else
					_unusedRecords.Add(i);
			}
		}

		private void CheckLoaded() {
			if (RequiresLoad)
				throw new InvalidOperationException($"{nameof(ClusteredDictionary<TKey, TValue>)} has not been loaded");
		}

		protected void NotifyLoading() {
			OnLoading();
			Loading?.Invoke(this);
		}

		protected void NotifyLoaded() {
			OnLoaded();
			Loaded?.Invoke(this);
		}

	}
}
