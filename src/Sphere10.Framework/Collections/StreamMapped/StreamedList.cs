using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sphere10.Framework {

	// TAKE CARE OF NULLS
	public class StreamedList<TItem, TRecord> : SingularListBase<TItem> where TRecord : IClusteredStreamRecord, new() {

		private readonly ClusteredStreamStorage<TRecord> _storage;
		private readonly IItemSerializer<TItem> _itemSerializer;
		private IEqualityComparer<TItem> _itemComparer;
		private readonly Endianness _endianness;
		private int _version;

		public StreamedList(Stream stream, int clusterSize, IItemSerializer<TItem> itemSerializer, IItemSerializer<TRecord> recordSerializer, IEqualityComparer<TItem> itemComparer = null, Endianness endianness = Endianness.LittleEndian) {
			Guard.ArgumentNotNull(stream, nameof(stream));
			Guard.ArgumentInRange(clusterSize, 1, int.MaxValue, nameof(clusterSize));
			Guard.ArgumentNotNull(itemSerializer, nameof(itemSerializer));
			Guard.ArgumentNotNull(recordSerializer, nameof(recordSerializer));
			_storage = new ClusteredStreamStorage<TRecord>(stream, clusterSize, recordSerializer, endianness);
			_itemSerializer = itemSerializer;
			_itemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
			_endianness = endianness;
			_version = 0;
		}

		public override int Count => _storage.Count;

		public override void Insert(int index, TItem item) {
			CheckIndex(index, allowAtEnd: true);
			_storage.InsertBytes(index, SerializeItem(item));
			MarkRecordNull(index, item == null);
			UpdateVersion();
		}

		public override void RemoveAt(int index) => _storage.Remove(EnsureSafe(index));

		public override TItem Read(int index) 
			=> IsRecordMarkedNull(index) ? default : _itemSerializer.Deserialize(_storage.ReadAll(EnsureSafe(index)), _endianness);

		public override int IndexOf(TItem item) {
			// TODO: if _storage keeps checksums, use that to quickly filter
			for (var i =0; i < _storage.Count; i++) {
				if (_itemComparer.Equals(item, IsRecordMarkedNull(i) ? default : _itemSerializer.Deserialize(_storage.ReadAll(i), _endianness)))
					return i;
			}
			return -1;
		}

		public override bool Contains(TItem item) => IndexOf(item) != -1;

		public override void Add(TItem item) {
			_storage.AddBytes(SerializeItem(item));
			if (item == null)
				MarkRecordNull(_storage.Count - 1, true);
			UpdateVersion();
		}

		public override void Update(int index, TItem item) {
			CheckIndex(index);
			var wasNull = IsRecordMarkedNull(index);
			_storage.UpdateBytes(index, SerializeItem(item));
			var isNull = item == null;
			if (wasNull != isNull)
				MarkRecordNull(index, isNull);
			UpdateVersion();
		}

		public override bool Remove(TItem item) {
			var index = IndexOf(item);
			if (index >= 0) {
				_storage.Remove(index);
				UpdateVersion();
				return true;
			}
			return false;
		}

		public override void Clear() {
			_storage.Clear();
			UpdateVersion();
		}

		public override void CopyTo(TItem[] array, int arrayIndex) {
			Guard.ArgumentNotNull(array, nameof(array));
			Guard.ArgumentInRange(arrayIndex, 0, Math.Max(0, array.Length - 1), nameof(array));
			var itemsToCopy = Math.Min(_storage.Count, array.Length - arrayIndex);
			for (var i = 0; i < itemsToCopy; i++)
				array[i + arrayIndex] = Read(i);
		}

		public override IEnumerator<TItem> GetEnumerator() {
			var version = _version;
			for (var i = 0; i < _storage.Count; i++) {
				if (_version != version)
					throw new InvalidOperationException("Collection was mutated during enumeration");
				yield return Read(i);
			}
		}

		private byte[] SerializeItem(TItem item) {
			if (item == null)
				return Array.Empty<byte>(); // note: null items are distinguished from empty items by a flag in record traits
			return _itemSerializer.Serialize(item, _endianness);
		}

		private void MarkRecordNull(int index, bool isNull) {
			var record = _storage.Records[index];
			record.Traits = record.Traits.CopyAndSetFlags(StreamRecordTraits.Bit0, isNull);
			_storage.UpdateRecord(index, record);
		}

		private bool IsRecordMarkedNull(int index) 
			=> _storage.Records[index].Traits.HasFlag(StreamRecordTraits.Bit0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateVersion() => Interlocked.Increment(ref _version);

	}

	public class StreamedList<TItem> : StreamedList<TItem, ClusteredStreamRecord> {
		public StreamedList(Stream stream, int clusterSize, IItemSerializer<TItem> itemSerializer, IEqualityComparer<TItem> itemComparer = null, Endianness endianness = Endianness.LittleEndian)
			: base(stream, clusterSize, itemSerializer, new ClusteredStreamRecordSerializer(), itemComparer, endianness) {
		}
	}
}
