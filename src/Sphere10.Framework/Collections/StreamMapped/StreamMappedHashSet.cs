using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

	/// <summary>
	/// A set whose items are mapped over a stream as a <see cref="StreamMappedList{TItem}"/>. A digest of the items are kept in the clustered record for fast lookup. 
	///
	/// </summary>
	/// <remarks>When deleting an item the underlying <see cref="ClusteredStreamRecord"/> is marked nullified but retained and re-used in later calls to <see cref="Add(TItem)"/>.</remarks>
	public class StreamMappedHashSet<TItem> : SetBase<TItem>, ILoadable {
		public event EventHandlerEx<object> Loading { add => _itemStore.Loading += value; remove => _itemStore.Loading -= value; }
		public event EventHandlerEx<object> Loaded { add => _itemStore.Loaded += value; remove => _itemStore.Loaded -= value; }

		private readonly IStreamMappedDictionary<byte[], TItem> _itemStore;
		private readonly IItemHasher<TItem> _hasher;

		public StreamMappedHashSet(Stream rootStream, int clusterSize, IItemSerializer<TItem> serializer, CHF chf, IEqualityComparer<TItem> comparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = Endianness.LittleEndian)
			: this(rootStream, clusterSize, serializer, new ItemHasher<TItem>(chf, serializer), comparer, policy, reservedRecords, endianness) {
		}

		public StreamMappedHashSet(Stream rootStream, int clusterSize, IItemSerializer<TItem> serializer, IItemHasher<TItem> hasher, IEqualityComparer<TItem> comparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = Endianness.LittleEndian)
			: this(
				new StreamMappedDictionarySK<byte[], TItem>(
					rootStream,
					clusterSize,
					new StaticSizeByteArraySerializer(hasher.DigestLength),
					serializer,
					new HashChecksum(),
					new ByteArrayEqualityComparer(),
					comparer,
					policy,
					reservedRecords,
					endianness
				),
				comparer,
				hasher
			) {
			Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in clustered dictionary implementations.");
		}

		public StreamMappedHashSet(IStreamMappedDictionary<byte[], TItem> itemStore, IEqualityComparer<TItem> comparer, IItemHasher<TItem> hasher) 
			: base(comparer ?? EqualityComparer<TItem>.Default) {
			_itemStore = itemStore;
			_hasher = hasher;
		}

		public override int Count => _itemStore.Count;

		public override bool IsReadOnly => _itemStore.IsReadOnly;

		public bool RequiresLoad => _itemStore.RequiresLoad;

		public IClusteredStorage Storage => _itemStore.Storage;

		public void Load() => _itemStore.Load();

		public override bool Add(TItem item) {
			Guard.ArgumentNotNull(item, nameof(item));
			var itemHash = _hasher.Hash(item);
			if (_itemStore.ContainsKey(itemHash))
				return false;
			_itemStore.Add(itemHash, item);
			return true;
		}

		public override bool Contains(TItem item) {
			Guard.ArgumentNotNull(item, nameof(item));
			return _itemStore.ContainsKey(_hasher.Hash(item));
		}

		public override bool Remove(TItem item) {
			Guard.ArgumentNotNull(item, nameof(item));
			var itemHash = _hasher.Hash(item);
			if (!_itemStore.TryFindKey(itemHash, out var index))
				return false;
			_itemStore.RemoveAt(index);
			return true;
		}

		public override void Clear() 
			=> _itemStore.Clear();

		public override void CopyTo(TItem[] array, int arrayIndex) 
			=> _itemStore.Values.CopyTo(array, arrayIndex);

		public override IEnumerator<TItem> GetEnumerator() 
			=> _itemStore.Values.GetEnumerator();

	}
}
