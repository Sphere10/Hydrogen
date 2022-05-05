﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Hydrogen {

	/// <summary>
	/// A dictionary whose keys and values are mapped over a stream as a <see cref="StreamMappedList{TItem}"/> of <see cref="KeyValuePair{TKey, TValue}"/>. A checksum of the key
	/// is kept in clustered record for fast lookup. 
	///
	/// IMPORTANT: Item deletions areWhen deleting a key, it's listing record is marked as unused and re-used later for efficiency.
	/// </summary>
	/// <typeparam name="TKey">The type of key stored in the dictionary</typeparam>
	/// <typeparam name="TValue">The type of value stored in the dictionary</typeparam>
	/// <remarks>When deleting an item the underlying <see cref="ClusteredStreamRecord"/> is marked nullified but retained and re-used in later calls to <see cref="Add(TKey,TValue)"/>.</remarks>
	// TODO: there are some memory-blowout lookups in this class that need to be refactored out (should be safe for non-huge dictionaries).
	public class StreamMappedDictionary<TKey, TValue> : DictionaryBase<TKey, TValue>, IStreamMappedDictionary<TKey, TValue> {
		public event EventHandlerEx<object> Loading;
		public event EventHandlerEx<object> Loaded;

		protected readonly IStreamMappedList<KeyValuePair<TKey, TValue>> KVPList;
		private readonly IEqualityComparer<TKey> _keyComparer;
		private readonly IEqualityComparer<TValue> _valueComparer;
		private readonly IItemChecksum<TKey> _keyChecksum;
		private readonly LookupEx<int, int> _checksumToIndexLookup;
		private readonly SortedList<int> _unusedRecords;

		public StreamMappedDictionary(Stream rootStream, int clusterSize, IItemSerializer<TKey> keySerializer = null, IItemSerializer<TValue> valueSerializer = null, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = Endianness.LittleEndian)
			: this(
				new StreamMappedList<KeyValuePair<TKey, TValue>>(
					rootStream,
					clusterSize,
					new KeyValuePairSerializer<TKey, TValue>(
						keySerializer ?? ItemSerializer<TKey>.Default,
						valueSerializer ?? ItemSerializer<TValue>.Default
					),
					new KeyValuePairEqualityComparer<TKey, TValue>(
						keyComparer,
						valueComparer
					),
					policy,
					0,
					reservedRecords,
					endianness
				),
				keyChecksum,
				keyComparer,
				valueComparer
			) {
			Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in {nameof(StreamMappedDictionary<TKey, TValue>)} implementations.");
		}

		public StreamMappedDictionary(IStreamMappedList<KeyValuePair<TKey, TValue>> kvpStore, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null) {
			Guard.ArgumentNotNull(kvpStore, nameof(kvpStore));
			_keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
			_valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
			KVPList = kvpStore;
			_keyChecksum = keyChecksum ?? new ActionChecksum<TKey>(DefaultCalculateKeyChecksum);
			_checksumToIndexLookup = new LookupEx<int, int>();
			_unusedRecords = new();
			RequiresLoad = KVPList.Storage.Records.Count > KVPList.Storage.Header.ReservedRecords;
		}

		public IClusteredStorage Storage => KVPList.Storage;

		protected IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs {
			get {
				for (var i = 0; i < KVPList.Count; i++) {
					if (IsUnusedRecord(i))
						continue;
					var (key, value) = KVPList[i];
					yield return new KeyValuePair<TKey, TValue>(key, value);
				}
			}
		}

		public bool RequiresLoad { get; private set; }

		public override int Count {
			get {
				CheckLoaded();
				return KVPList.Count - _unusedRecords.Count;
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
			if (Storage.IsNull(KVPList.Storage.Header.ReservedRecords + index))
				throw new InvalidOperationException($"Stream record {index} is null");
			using var scope = Storage.Open(KVPList.Storage.Header.ReservedRecords + index);
			var reader = new EndianBinaryReader(EndianBitConverter.For(Storage.Endianness), scope.Stream);
			return ((KeyValuePairSerializer<TKey, TValue>)KVPList.ItemSerializer).DeserializeKey(scope.Record.Size, reader);
		}

		public TValue ReadValue(int index) {
			if (Storage.IsNull(KVPList.Storage.Header.ReservedRecords + index))
				throw new InvalidOperationException($"Stream record {index} is null");
				
			using var scope = Storage.Open(KVPList.Storage.Header.ReservedRecords + index);
			var reader = new EndianBinaryReader(EndianBitConverter.For(Storage.Endianness), scope.Stream);
			return ((KeyValuePairSerializer<TKey, TValue>)KVPList.ItemSerializer).DeserializeValue(scope.Record.Size, reader);
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
				RemoveAt(index);
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
			KVPList.Clear();
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

		public bool TryFindKey(TKey key, out int index) {
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

		public bool TryFindValue(TKey key, out int index, out TValue value) {
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

		public override void Add(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			Guard.ArgumentNotNull(item, nameof(item));
			CheckLoaded();
			var kvp = new KeyValuePair<TKey, TValue>(item.Key, item.Value);
			if (TryFindKey(item.Key, out var index)) {
				KVPList.Update(index, kvp);
			} else {
				AddInternal(kvp);
			}
		}

		public override bool Remove(KeyValuePair<TKey, TValue> item) {
			Guard.ArgumentNotNull(item, nameof(item));
			CheckLoaded();
			if (TryFindValue(item.Key, out var index, out var value)) {
				if (_valueComparer.Equals(item.Value, value)) {
					RemoveAt(index);
					return true;
				}
			}
			return false;
		}

		public void RemoveAt(int index) {
			// We don't delete the instance, we mark is as unused. Use Shrink to intelligently remove unused records.
			var kvp = KeyValuePair.Create(default(TKey), default(TValue));
			using var scope = KVPList.EnterUpdateScope(index, kvp);

			// Mark record as unused
			Guard.Ensure(scope.Record.Traits.HasFlag(ClusteredStreamTraits.IsUsed), "Record not in used state");
			_checksumToIndexLookup.Remove(scope.Record.KeyChecksum, index);
			scope.Record.KeyChecksum = -1;
			scope.Record.Traits = scope.Record.Traits.CopyAndSetFlags(ClusteredStreamTraits.IsUsed, false);
			_unusedRecords.Add(index);

			// note: scope Dispose ends up updating the record
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
				KVPList.RemoveAt(index);
				_unusedRecords.RemoveAt(i);
			}
		}

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			CheckLoaded();
			return KVPList
			  .Where((_, i) => !_unusedRecords.Contains(i))
			  .Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value))
			  .GetEnumerator();
		}

		protected override IEnumerator<TKey> GetKeysEnumerator() {
			CheckLoaded();
			return Enumerable.Range(0, KVPList.Count)
			   .Where(i => !_unusedRecords.Contains(i))
			   .Select(ReadKey)
			   .GetEnumerator();
		}

		protected override IEnumerator<TValue> GetValuesEnumerator() {
			CheckLoaded();
			return Enumerable.Range(0, KVPList.Count)
				.Where(i => !_unusedRecords.Contains(i))
				.Select(ReadValue)
				.GetEnumerator();
		}

		protected virtual void OnLoading() {
		}

		protected virtual void OnLoaded() {
		}

		private void AddInternal(KeyValuePair<TKey, TValue> item) {
			int index;
			ClusteredStreamScope scope;
			if (_unusedRecords.Any()) {
				index = ConsumeUnusedRecord();
				scope = KVPList.EnterUpdateScope(index, item);
			} else {
				scope = KVPList.EnterAddScope(item);
				index = Count - 1;
			}

			// Mark record as used
			using (scope) {
				Guard.Ensure(!scope.Record.Traits.HasFlag(ClusteredStreamTraits.IsUsed), "Record not in unused state");
				scope.Record.KeyChecksum = _keyChecksum.Calculate(item.Key);
				scope.Record.Traits = scope.Record.Traits.CopyAndSetFlags(ClusteredStreamTraits.IsUsed, true);
				_checksumToIndexLookup.Add(scope.Record.KeyChecksum, index);
				// note: scope Dispose ends up updating the record
			}

		}

		private void UpdateInternal(int index, KeyValuePair<TKey, TValue> item) {
			// Updating value only, records (and checksum) don't change  when updating
			using var scope = KVPList.EnterUpdateScope(index, item);
			scope.Record.Traits = scope.Record.Traits.CopyAndSetFlags(ClusteredStreamTraits.IsUsed, true);
			scope.Record.KeyChecksum = _keyChecksum.Calculate(item.Key);
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
			for (var i = 0; i < KVPList.Count; i++) {
				var record = KVPList.Storage.Records[KVPList.Storage.Header.ReservedRecords + i];
				if (record.Traits.HasFlag(ClusteredStreamTraits.IsUsed))
					_checksumToIndexLookup.Add(record.KeyChecksum, i);
				else
					_unusedRecords.Add(i);
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
}
