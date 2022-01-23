using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Sphere10.Framework {

	/// <summary>
	/// A dictionary whose keys and values are mapped over a stream. When deleting a key, it's listing record is marked as unused and re-used later for efficiency.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <typeparam name="TRecord"></typeparam>
	/// <remarks>This is useful when the underlying KVP store isn't efficient at deletion. When deleting an item, it's record is marked as available and re-used later.</remarks>
	public class StreamPersistedDictionary<TKey, TValue, THeader, TRecord> : DictionaryBase<TKey, TValue>
		where THeader : IStreamStorageHeader
		where TRecord : IStreamKeyRecord, new() {

		private readonly IItemSerializer<TValue> _valueSerializer;
		private readonly Endianness _endianness;
		private readonly IStreamPersistedList<KeyValuePair<TKey, byte[]>, THeader, TRecord> _kvpStore;
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
		protected StreamPersistedDictionary(IStreamPersistedList<KeyValuePair<TKey, byte[]>, THeader, TRecord> kvpStore, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer = null, Endianness endianess = Endianness.LittleEndian) {
			_keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
			_kvpStore = kvpStore;
			_valueSerializer = valueSerializer;
			_endianness = endianess;
			_checksumToIndexLookup = new LookupEx<int, int>();
			_unusedRecords = new();
		}

		public override ICollection<TKey> Keys
			=> _kvpStore
				  .Where((_, i) => !IsUnusedRecord(i))
				  .Select(kvp => kvp.Key)
				  .ToList();

		public override ICollection<TValue> Values
			=> _kvpStore
				  .Where((_, i) => !IsUnusedRecord(i))
				  .Select(kvp => DeserializeValue(kvp.Value))
				  .ToList();

		protected IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs =>
			_kvpStore
				.Where((_, i) => !IsUnusedRecord(i))
				.Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, DeserializeValue(kvp.Value)));

		public override void Add(TKey key, TValue value) {
			Guard.ArgumentNotNull(key, nameof(key));
			var newBytes = SerializeValue(value);
			var newStorageKVP = new KeyValuePair<TKey, byte[]>(key, newBytes);
			if (TryFindKVP(key, out var index, out _)) {
				// Updating value only, records (and checksum) don't change  when updating
				_kvpStore[index] = newStorageKVP;
				var record = _kvpStore.Storage.Records[index];
				record.Traits = record.Traits.CopyAndSetFlags(StreamRecordTraits.IsUsed, true);
				_kvpStore.Storage.UpdateRecord(index, record);
			} else {
				AddInternal(newStorageKVP);
			}
		}

		public override bool Remove(TKey key) {
			Guard.ArgumentNotNull(key, nameof(key));
			if (TryFindKVP(key, out var index, out _)) {
				RemoveInternal(key, index);
				return true;
			}
			return false;
		}

		public override bool ContainsKey(TKey key) {
			Guard.ArgumentNotNull(key, nameof(key));
			return
				_checksumToIndexLookup[CalculateKeyChecksum(key)]
					.Where(index => !IsUnusedRecord(index))
					.Select(_kvpStore.Read)
					.Any(item => _keyComparer.Equals(item.Key, key));
		}

		public override void Clear() {
			_kvpStore.Clear();
			_checksumToIndexLookup.Clear();
			_unusedRecords.Clear();
		}

		public override int Count => _kvpStore.Count - _unusedRecords.Count;

		public override bool IsReadOnly { get; }

		public override bool TryGetValue(TKey key, out TValue value) {
			Guard.ArgumentNotNull(key, nameof(key));
			if (TryFindKVP(key, out _, out var valueBytes)) {
				value = DeserializeValue(valueBytes);
				return true;
			}
			value = default;
			return false;
		}

		public override void Add(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			Guard.ArgumentNotNull(item, nameof(item));
			var newBytes = SerializeValue(item.Value);
			var newStorageKVP = new KeyValuePair<TKey, byte[]>(item.Key, newBytes);
			if (TryFindKVP(item.Key, out var index, out _)) {
				_kvpStore.Update(index, newStorageKVP);
			} else {
				AddInternal(newStorageKVP);
			}
		}

		public override bool Remove(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			if (TryFindKVP(item.Key, out var index, out var kvpValueBytes)) {
				var serializedValue = SerializeValue(item.Value);
				if (ByteArrayEqualityComparer.Instance.Equals(serializedValue, kvpValueBytes)) {
					RemoveInternal(item.Key, index);
					return true;
				}
			}
			return false;
		}

		public override bool Contains(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			if (!TryFindKVP(item.Key, out _, out var valueBytes))
				return false;
			var itemValueBytes = SerializeValue(item.Value);
			return ByteArrayEqualityComparer.Instance.Equals(valueBytes, itemValueBytes);
		}

		public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			=> KeyValuePairs.ToArray().CopyTo(array, arrayIndex);


		public void Shrink() {
			// delete all unused records from _kvpStore
			// deletes item right to left
			// possible optimization: a connected neighbourhood of unused records can be deleted 
			for (var i = _unusedRecords.Count - 1; i >= 0; i--) {
				var index = _unusedRecords[i];
				_kvpStore.RemoveAt(i);
				_unusedRecords.RemoveAt(i);
			}
		}

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return _kvpStore
			  .Where((_, i) => !_unusedRecords.Contains(i))
			  .Select(storageKVP => new KeyValuePair<TKey, TValue>(storageKVP.Key, DeserializeValue(storageKVP.Value)))
			  .GetEnumerator();
		}

		private bool TryFindKVP(TKey key, out int index, out byte[] valueBytes) {
			Guard.ArgumentNotNull(key, nameof(key));
			foreach (var i in _checksumToIndexLookup[CalculateKeyChecksum(key)]) {
				var kvp = _kvpStore.Read(i);
				if (_keyComparer.Equals(key, kvp.Key)) {
					index = i;
					valueBytes = kvp.Value;
					return true;
				}
			}
			index = -1;
			valueBytes = default;
			return false;
		}

		private void AddInternal(KeyValuePair<TKey, byte[]> item) {
			if (_unusedRecords.Any()) {
				var index = ConsumeUnusedrecord();
				MarkRecordAsUsed(index, item.Key);
				_kvpStore.Update(index, item);
			} else {
				_kvpStore.Add(item);
				_checksumToIndexLookup.Add(CalculateKeyChecksum(item.Key), _kvpStore.Count - 1);
			}
		}

		private void RemoveInternal(TKey key, int index) {
			// We don't delete the instance, we mark is as unused. Use Shrink to intelligently remove unused records.
			var kvp = KeyValuePair.Create(key, null as byte[]);
			_kvpStore.Update(index, kvp);
			MarkRecordAsUnused(index);  // record has to be updated after _kvpStore update since it removes/creates under the hood
		}

		private StreamKeyRecord NewRecordInstance(object source, KeyValuePair<TKey, byte[]> item, int itemSizeBytes, int clusterStartIndex)
			=> new() {
				Size = itemSizeBytes,
				StartCluster = clusterStartIndex,
				Traits = StreamRecordTraits.IsUsed,
				KeyChecksum = CalculateKeyChecksum(item.Key)
			};

		private bool IsUnusedRecord(int index) => _unusedRecords.Contains(index);


		private int ConsumeUnusedrecord() {
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

		private byte[] SerializeValue(TValue value)
			=> value != null ? _valueSerializer.Serialize(value, _endianness) : null;

		private TValue DeserializeValue(byte[] valueBytes)
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
	}
}
