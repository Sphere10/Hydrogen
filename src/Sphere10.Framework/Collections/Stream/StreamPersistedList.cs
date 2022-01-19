using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sphere10.Framework {

	/// <summary>
	/// A list whose items are persisted via an <see cref="IStreamStorage{THeader,TRecord}"/>.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="THeader"></typeparam>
	/// <typeparam name="TRecord"></typeparam>
	public class StreamPersistedList<TItem, THeader, TRecord> : SingularListBase<TItem>
		where THeader : ClusteredStorageHeader
		where TRecord : IClusteredRecord {

		private readonly IItemSerializer<TItem> _itemSerializer;
		private readonly IEqualityComparer<TItem> _itemComparer;
		private readonly Endianness _endianness;
		private int _version;

		protected StreamPersistedList(IStreamStorage<THeader, TRecord> storage, IItemSerializer<TItem> itemSerializer, IEqualityComparer<TItem> itemComparer = null) {
			Guard.ArgumentNotNull(storage, nameof(storage));
			Guard.ArgumentNotNull(itemSerializer, nameof(itemSerializer));
			Storage = storage;
			_itemSerializer = itemSerializer;
			_itemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
			_version = 0;
		}

		public override int Count => Storage.Count;

		public IReadOnlyList<TRecord> StreamRecords => Storage.Records;

		internal IStreamStorage<THeader, TRecord> Storage { get; }

		public override void Insert(int index, TItem item) {
			CheckIndex(index, allowAtEnd: true);
			Storage.InsertBytes(index, SerializeItem(item));
			MarkRecordNull(index, item == null);
			UpdateVersion();
		}

		public override void RemoveAt(int index) => Storage.Remove(EnsureSafe(index));

		public override TItem Read(int index)
			=> IsRecordMarkedNull(index) ? default : _itemSerializer.Deserialize(Storage.ReadAll(EnsureSafe(index)), _endianness);

		public override int IndexOf(TItem item) {
			// TODO: if _storage keeps checksums, use that to quickly filter
			for (var i = 0; i < Storage.Count; i++) {
				if (_itemComparer.Equals(item, IsRecordMarkedNull(i) ? default : _itemSerializer.Deserialize(Storage.ReadAll(i), _endianness)))
					return i;
			}
			return -1;
		}

		public override bool Contains(TItem item) => IndexOf(item) != -1;

		public override void Add(TItem item) {
			Storage.AddBytes(SerializeItem(item));
			if (item == null)
				MarkRecordNull(Storage.Count - 1, true);
			UpdateVersion();
		}

		public override void Update(int index, TItem item) {
			CheckIndex(index);
			var wasNull = IsRecordMarkedNull(index);
			Storage.UpdateBytes(index, SerializeItem(item));
			var isNull = item == null;
			if (wasNull != isNull)
				MarkRecordNull(index, isNull);
			UpdateVersion();
		}

		public override bool Remove(TItem item) {
			var index = IndexOf(item);
			if (index >= 0) {
				Storage.Remove(index);
				UpdateVersion();
				return true;
			}
			return false;
		}

		public override void Clear() {
			Storage.Clear();
			UpdateVersion();
		}

		public override void CopyTo(TItem[] array, int arrayIndex) {
			Guard.ArgumentNotNull(array, nameof(array));
			Guard.ArgumentInRange(arrayIndex, 0, Math.Max(0, array.Length - 1), nameof(array));
			var itemsToCopy = Math.Min(Storage.Count, array.Length - arrayIndex);
			for (var i = 0; i < itemsToCopy; i++)
				array[i + arrayIndex] = Read(i);
		}

		public override IEnumerator<TItem> GetEnumerator() {
			var version = _version;
			for (var i = 0; i < Storage.Count; i++) {
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
			var record = Storage.Records[index];
			record.Traits = record.Traits.CopyAndSetFlags(StreamRecordTraits.Bit0, isNull);
			Storage.UpdateRecord(index, record);
		}

		private bool IsRecordMarkedNull(int index)
			=> Storage.Records[index].Traits.HasFlag(StreamRecordTraits.Bit0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateVersion() => Interlocked.Increment(ref _version);


	}

}

