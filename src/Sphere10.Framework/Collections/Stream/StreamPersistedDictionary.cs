using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Sphere10.Framework.Collections.Stream;

namespace Sphere10.Framework {

	/// <summary>
	/// A dictionary whose keys and values are mapped over a stream. When deleting a key, it's listing record is marked as unused and re-used later for efficiency.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <typeparam name="TRecord"></typeparam>
	/// <remarks>This is useful when the underlying KVP store isn't efficient at deletion. When deleting an item, it's record is marked as available and re-used later.</remarks>
	// TODO: there are some memory-blowout lookups in this class that need to be refactored out (should be safe for non-huge dictionaries).
	public class StreamPersistedDictionary<TKey, TValue, THeader, TRecord> : DictionaryBase<TKey, TValue>, ILoadable
		where THeader : IStreamStorageHeader
		where TRecord : IStreamKeyRecord {
		public event EventHandlerEx<object> Loading;
		public event EventHandlerEx<object> Loaded;

		private readonly IItemSerializer<TValue> _valueSerializer;
		private readonly Endianness _endianness;
		private readonly IStreamKeyValueStore<TKey, THeader, TRecord> _kvpStore;
		private readonly IEqualityComparer<TKey> _keyComparer;
		private readonly LookupEx<int, int> _checksumToIndexLookup;
		private readonly SortedList<int> _unusedRecords;

		/// <summary>
		/// Constructs a dictionary which uses argument <see cref="kvpStore"/> clustered list to storage it's key-value pairs.
		/// </summary>
		/// <param name="kvpStore"></param>
		/// <param name="valueSerializer"></param>
		/// <param name="keyComparer"></param>
		/// <param name="endianess"></param>
		protected StreamPersistedDictionary(IStreamKeyValueStore<TKey, THeader, TRecord> kvpStore, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer = null, Endianness endianess = Endianness.LittleEndian) {
			_keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
			_kvpStore = kvpStore;
			_valueSerializer = valueSerializer;
			_endianness = endianess;
			_checksumToIndexLookup = new LookupEx<int, int>();
			_unusedRecords = new();
			RequiresLoad = kvpStore.Storage.Records.Count > 0;
		}

		public IStreamStorage<THeader, TRecord> Storage => _kvpStore.Storage;

		public override ICollection<TKey> Keys
			=> _kvpStore
				  .Where((_, i) => !IsUnusedRecord(i))
				  .Select(kvp => kvp.Key)
				  .ToList();

		public override ICollection<TValue> Values
			=> _kvpStore
				  .Where((_, i) => !IsUnusedRecord(i))
				  .Select(kvp => ConvertValueFromBytes(kvp.Value))
				  .ToList();

		protected IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs =>
			_kvpStore
				.Where((_, i) => !IsUnusedRecord(i))
				.Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, ConvertValueFromBytes(kvp.Value)));

		public bool RequiresLoad { get; private set; }

		public void Load() {
			RefreshChecksumToIndexLookup();
			RequiresLoad = false;
		}

		public override void Add(TKey key, TValue value) {
			Guard.ArgumentNotNull(key, nameof(key));
			CheckLoaded();
			var newBytes = ConvertValueToBytes(value);
			var newStorageKVP = new KeyValuePair<TKey, byte[]>(key, newBytes);
			if (TryFindKey(key, out var index)) {
				UpdateInternal(index, newStorageKVP);
			} else {
				AddInternal(newStorageKVP);
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
				_checksumToIndexLookup[CalculateKeyChecksum(key)]
					.Where(index => !IsUnusedRecord(index))
					.Select(_kvpStore.ReadKey)
					.Any(item => _keyComparer.Equals(item, key));
		}

		public override void Clear() {
			// Load not required
			_kvpStore.Clear();
			_checksumToIndexLookup.Clear();
			_unusedRecords.Clear();
		}

		public override int Count => _kvpStore.Count - _unusedRecords.Count;  // Load not required

		public override bool IsReadOnly => false;

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
			var newStorageKVP = new KeyValuePair<TKey, byte[]>(item.Key, newBytes);
			if (TryFindKVP(item.Key, out var index, out _)) {
				_kvpStore.Update(index, newStorageKVP);
			} else {
				AddInternal(newStorageKVP);
			}
		}

		public override bool Remove(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			CheckLoaded();
			if (TryFindKVP(item.Key, out var index, out var kvpValueBytes)) {
				var serializedValue = ConvertValueToBytes(item.Value);
				if (ByteArrayEqualityComparer.Instance.Equals(serializedValue, kvpValueBytes)) {
					RemoveInternal(item.Key, index);
					return true;
				}
			}
			return false;
		}

		public override bool Contains(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			CheckLoaded();
			if (!TryFindKVP(item.Key, out _, out var valueBytes))
				return false;
			var itemValueBytes = ConvertValueToBytes(item.Value);
			return ByteArrayEqualityComparer.Instance.Equals(valueBytes, itemValueBytes);
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
			for (var i = _unusedRecords.Count - 1; i >= 0; i--) {
				var index = _unusedRecords[i];
				_kvpStore.RemoveAt(i);
				_unusedRecords.RemoveAt(i);
			}
		}

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			CheckLoaded();
			return _kvpStore
			  .Where((_, i) => !_unusedRecords.Contains(i))
			  .Select(storageKVP => new KeyValuePair<TKey, TValue>(storageKVP.Key, ConvertValueFromBytes(storageKVP.Value)))
			  .GetEnumerator();
		}

		private bool TryFindKVP(TKey key, out int index, out byte[] valueBytes) {
			Debug.Assert(key != null);
			foreach (var i in _checksumToIndexLookup[CalculateKeyChecksum(key)]) {
				var candidateKey = _kvpStore.ReadKey(i);
				if (_keyComparer.Equals(candidateKey,key)) {
					index = i;
					valueBytes = _kvpStore.ReadValue(i);
					return true;
				}
			}
			index = -1;
			valueBytes = default;
			return false;
		}

		private bool TryFindKey(TKey key, out int index) {
			Debug.Assert(key != null);
			foreach (var i in _checksumToIndexLookup[CalculateKeyChecksum(key)]) {
				var candidateKey = _kvpStore.ReadKey(i);
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
			foreach (var i in _checksumToIndexLookup[CalculateKeyChecksum(key)]) {
				var candidateKey = _kvpStore.ReadKey(i);
				if (_keyComparer.Equals(candidateKey, key)) {
					index = i;
					valueBytes = _kvpStore.ReadValue(index);
					return true;
				}
			}
			index = -1;
			valueBytes = default;
			return false;
		}

		private void AddInternal(KeyValuePair<TKey, byte[]> item) {
			int index;
			if (_unusedRecords.Any()) {
				index = ConsumeUnusedRecord();
				_kvpStore.Update(index, item);
			} else {
				_kvpStore.Add(item);
				index = Count - 1;
			}
			MarkRecordAsUsed(index, item.Key);
		}

		private void UpdateInternal(int index, KeyValuePair<TKey, byte[]> item) {
			// Updating value only, records (and checksum) don't change  when updating
			_kvpStore[index] = item;
			var record = _kvpStore.Storage.Records[index];
			record.Traits &= StreamRecordTraits.IsUsed;
			record.KeyChecksum = CalculateKeyChecksum(item.Key);
			_kvpStore.Storage.UpdateRecord(index, record);

		}

		private void RemoveInternal(TKey key, int index) {
			// We don't delete the instance, we mark is as unused. Use Shrink to intelligently remove unused records.
			var kvp = KeyValuePair.Create(key, null as byte[]);
			_kvpStore.Update(index, kvp);
			MarkRecordAsUnused(index);  // record has to be updated after _kvpStore update since it removes/creates under the hood
		}

		private bool IsUnusedRecord(int index) => _unusedRecords.Contains(index);

		private int ConsumeUnusedRecord() {
			var index = _unusedRecords[0];
			_unusedRecords.RemoveAt(0);
			return index;
		}

		private void MarkRecordAsUnused(int index) {
			var record = _kvpStore.Storage.Records[index];
			Guard.Ensure(record.Traits.HasFlag(StreamRecordTraits.IsUsed), "Record not in used state");
			_checksumToIndexLookup.Remove(record.KeyChecksum, index);
			record.KeyChecksum = -1;
			record.Traits = record.Traits.CopyAndSetFlags(StreamRecordTraits.IsUsed, false);
			_kvpStore.Storage.UpdateRecord(index, record);
			_unusedRecords.Add(index);
		}

		private void MarkRecordAsUsed(int index, TKey key) {
			var record = _kvpStore.Storage.Records[index];
			Guard.Ensure(!record.Traits.HasFlag(StreamRecordTraits.IsUsed), "Record not in unused state");
			record.KeyChecksum = CalculateKeyChecksum(key);
			record.Traits = record.Traits.CopyAndSetFlags(StreamRecordTraits.IsUsed, true);
			_kvpStore.Storage.UpdateRecord(index, record);
			_checksumToIndexLookup.Add(record.KeyChecksum, index);
		}

		private byte[] ConvertValueToBytes(TValue value)
			=> value != null ? _valueSerializer.Serialize(value, _endianness) : null;

		private TValue ConvertValueFromBytes(byte[] valueBytes)
			=> valueBytes != null ? _valueSerializer.Deserialize(valueBytes, _endianness) : default;

		private int CalculateKeyChecksum(TKey key)
			=> _keyComparer.GetHashCode(key);

		private void RefreshChecksumToIndexLookup() {
			_checksumToIndexLookup.Clear();
			_unusedRecords.Clear();
			for (var i = 0; i < _kvpStore.Storage.Records.Count; i++) {
				var record = _kvpStore.Storage.Records[i];
				if (record.Traits.HasFlag(StreamRecordTraits.IsUsed)) 
					_checksumToIndexLookup.Add(record.KeyChecksum, i);
				else
					_unusedRecords.Add(i);
			}
		}

		protected void CheckLoaded() {
			if (RequiresLoad)
				throw new InvalidOperationException("StreamPersistedDictionary has not been loaded");
		}
	}
}
