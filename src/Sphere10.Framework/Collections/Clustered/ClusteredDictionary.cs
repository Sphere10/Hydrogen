using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A dictionary whose keys and values are mapped over a stream as a <see cref="ClusteredList{TItem}"/> of <see cref="KeyValuePair{TKey, TValue}"/>. A checksum of the key
	/// is kept in clustered record for fast lookup. 
	///
	/// IMPORTANT: Item deletions areWhen deleting a key, it's listing record is marked as unused and re-used later for efficiency.
	/// </summary>
	/// <typeparam name="TKey">The type of key stored in the dictionary</typeparam>
	/// <typeparam name="TValue">The type of value stored in the dictionary</typeparam>
	/// <remarks>When deleting an item the underlying <see cref="ClusteredStorageRecord"/> is marked nullified but retained and re-used in later calls to <see cref="Add(TKey,TValue)"/>.</remarks>
	// TODO: there are some memory-blowout lookups in this class that need to be refactored out (should be safe for non-huge dictionaries).
	public class ClusteredDictionary<TKey, TValue> : DictionaryBase<TKey, TValue>, IClusteredDictionary<TKey, TValue>, ILoadable {
		public event EventHandlerEx<object> Loading;
		public event EventHandlerEx<object> Loaded;

		private readonly Endianness _endianness;
		private readonly IClusteredList<KeyValuePair<TKey, TValue>> _kvpStore;
		private readonly IEqualityComparer<TKey> _keyComparer;
		private readonly IEqualityComparer<TValue> _valueComparer;
		private readonly IItemChecksum<TKey> _keyChecksum;
		private readonly LookupEx<int, int> _checksumToIndexLookup;
		private readonly SortedList<int> _unusedRecords;

		public ClusteredDictionary(Stream rootStream, int clusterSize, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, Endianness endianness = Endianness.LittleEndian)
			: this(
				new ClusteredList<KeyValuePair<TKey, TValue>>(
					rootStream,
					clusterSize,
					new KeyValuePairSerializer<TKey, TValue>(
						keySerializer,
						valueSerializer
					),
					new KeyValuePairEqualityComparer<TKey, TValue>(
						keyComparer,
						valueComparer
					),
					policy,
					0,
					endianness
				),
				keyChecksum,
				keyComparer,
				valueComparer,
				endianness
			) {
			Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in {nameof(ClusteredDictionary<TKey, TValue>)} implementations.");
		}

		public ClusteredDictionary(IClusteredList<KeyValuePair<TKey, TValue>> kvpStore, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null, Endianness endianess = Endianness.LittleEndian) {
			Guard.ArgumentNotNull(kvpStore, nameof(kvpStore));
			_keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
			_valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
			_kvpStore = kvpStore;
			_keyChecksum = keyChecksum ?? new ActionChecksum<TKey>(DefaultCalculateKeyChecksum);
			_endianness = endianess;
			_checksumToIndexLookup = new LookupEx<int, int>();
			_unusedRecords = new();
			RequiresLoad = _kvpStore.Storage.Records.Count > 0;
		}

		public IClusteredStorage Storage => _kvpStore.Storage;

		protected IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs {
			get {
				for (var i = 0; i < _kvpStore.Storage.Records.Count; i++) {
					if (IsUnusedRecord(i))
						continue;
					var (key, value) = _kvpStore[i];
					yield return new KeyValuePair<TKey, TValue>(key, value);
				}
			}
		}

		public bool RequiresLoad { get; private set; }

		public override int Count {
			get {
				CheckLoaded();
				return _kvpStore.Count - _unusedRecords.Count;
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
			using var scope = Storage.Open(index);
			var reader = new EndianBinaryReader(EndianBitConverter.For(_endianness), scope.Stream);
			return ((KeyValuePairSerializer<TKey, TValue>)_kvpStore.ItemSerializer).DeserializeKey(scope.Record.Size, reader);
		}

		public TValue ReadValue(int index) {
			if (Storage.IsNull(index))
				throw new InvalidOperationException($"Stream record {index} is null");
			using var scope = Storage.Open(index);
			var reader = new EndianBinaryReader(EndianBitConverter.For(_endianness), scope.Stream);
			return ((KeyValuePairSerializer<TKey, TValue>)_kvpStore.ItemSerializer).DeserializeValue(scope.Record.Size, reader);
		}

		public override void Add(TKey key, TValue value) {
			Guard.ArgumentNotNull(key, nameof(key));
			CheckLoaded();
			var kvp = new KeyValuePair<TKey, TValue>(key, value);
			if (TryFindKey(key, out var index)) {
				UpdateInternal(index, kvp);
			} else {
				AddInternal(kvp);
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
			_kvpStore.Clear();
			_checksumToIndexLookup.Clear();
			_unusedRecords.Clear();
		}

		public override bool TryGetValue(TKey key, out TValue value) {
			Guard.ArgumentNotNull(key, nameof(key));
			CheckLoaded();
			if (TryFindValue(key, out _, out value)) {
				return true;
			}
			value = default;
			return false;
		}

		public override void Add(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			Guard.ArgumentNotNull(item, nameof(item));
			CheckLoaded();
			var kvp = new KeyValuePair<TKey, TValue>(item.Key, item.Value);
			if (TryFindKey(item.Key, out var index)) {
				_kvpStore.Update(index, kvp);
			} else {
				AddInternal(kvp);
			}
		}

		public override bool Remove(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			CheckLoaded();
			if (TryFindValue(item.Key, out var index, out var value)) {
				if (_valueComparer.Equals(item.Value, value)) {
					RemoveInternal(item.Key, index);
					return true;
				}
			}
			return false;
		}

		public override bool Contains(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			CheckLoaded();
			if (!TryFindValue(item.Key, out _, out var value))
				return false;
			return _valueComparer.Equals(value, item.Value);
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
			  .Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value))
			  .GetEnumerator();
		}

		protected override IEnumerator<TKey> GetKeysEnumerator() {
			CheckLoaded();
			return Enumerable.Range(0, _kvpStore.Count)
			   .Where(i => !_unusedRecords.Contains(i))
			   .Select(ReadKey)
			   .GetEnumerator();
		}

		protected override IEnumerator<TValue> GetValuesEnumerator() {
			CheckLoaded();
			return Enumerable.Range(0, _kvpStore.Count)
				.Where(i => !_unusedRecords.Contains(i))
				.Select(ReadValue)
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

		private bool TryFindValue(TKey key, out int index, out TValue value) {
			Debug.Assert(key != null);
			foreach (var i in _checksumToIndexLookup[_keyChecksum.Calculate(key)]) {
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

		private void AddInternal(KeyValuePair<TKey, TValue> item) {
			int index;
			ClusteredStorageScope scope;
			if (_unusedRecords.Any()) {
				index = ConsumeUnusedRecord();
				scope = _kvpStore.EnterUpdateScope(index, item);
			} else {
				scope = _kvpStore.EnterAddScope(item);
				index = Count - 1;
			}

			// Mark record as used
			using (scope) {
				Guard.Ensure(!scope.Record.Traits.HasFlag(ClusteredStorageRecordTraits.IsUsed), "Record not in unused state");
				scope.Record.KeyChecksum = _keyChecksum.Calculate(item.Key);
				scope.Record.Traits = scope.Record.Traits.CopyAndSetFlags(ClusteredStorageRecordTraits.IsUsed, true);
				_checksumToIndexLookup.Add(scope.Record.KeyChecksum, index);
				// note: scope Dispose ends up updating the record
			}

		}

		private void UpdateInternal(int index, KeyValuePair<TKey, TValue> item) {
			// Updating value only, records (and checksum) don't change  when updating
			using var scope = _kvpStore.EnterUpdateScope(index, item);
			scope.Record.Traits = scope.Record.Traits.CopyAndSetFlags(ClusteredStorageRecordTraits.IsUsed, true);
			scope.Record.KeyChecksum = _keyChecksum.Calculate(item.Key);
		}

		private void RemoveInternal(TKey key, int index) {
			// We don't delete the instance, we mark is as unused. Use Shrink to intelligently remove unused records.
			var kvp = KeyValuePair.Create(default(TKey), default(TValue));
			using var scope = _kvpStore.EnterUpdateScope(index, kvp);

			// Mark record as unused
			Guard.Ensure(scope.Record.Traits.HasFlag(ClusteredStorageRecordTraits.IsUsed), "Record not in used state");
			_checksumToIndexLookup.Remove(scope.Record.KeyChecksum, index);
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

		private int DefaultCalculateKeyChecksum(TKey key)
			=> _keyComparer.GetHashCode(key);

		private void RefreshChecksumToIndexLookup() {
			_checksumToIndexLookup.Clear();
			_unusedRecords.Clear();
			for (var i = 0; i < _kvpStore.Storage.Records.Count; i++) {
				var record = _kvpStore.Storage.Records[i];
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
