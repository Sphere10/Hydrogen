using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen;

public class TransactionalHashSet<TItem> : StreamMappedHashSet<TItem>, ITransactionalHashSet<TItem> {
	
	public event EventHandlerEx<object> Committing { add => _internalTransactionalDictionary.Committing += value; remove => _internalTransactionalDictionary.Committing -= value; }
	public event EventHandlerEx<object> Committed { add => _internalTransactionalDictionary.Committed += value; remove => _internalTransactionalDictionary.Committed -= value; }
	public event EventHandlerEx<object> RollingBack { add => _internalTransactionalDictionary.RollingBack += value; remove => _internalTransactionalDictionary.RollingBack -= value; }
	public event EventHandlerEx<object> RolledBack { add => _internalTransactionalDictionary.RolledBack += value; remove => _internalTransactionalDictionary.RolledBack -= value; }

	internal readonly ITransactionalDictionary<byte[], TItem> _internalTransactionalDictionary;

	public TransactionalHashSet(string filename, string uncommittedPageFileDir, IItemSerializer<TItem> serializer, CHF chf = CHF.SHA2_256, IItemChecksummer<byte[]> keyChecksum = null, IEqualityComparer<TItem> comparer = null,
	                            int transactionalPageSize = HydrogenDefaults.TransactionalPageSize, long maxMemory = HydrogenDefaults.MaxMemoryPerCollection, int clusterSize = HydrogenDefaults.ClusterSize,
	                            ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = HydrogenDefaults.Endianness, bool readOnly = false)
		: this(
			new TransactionalStream(filename, uncommittedPageFileDir, transactionalPageSize, maxMemory, readOnly, readOnly),
			clusterSize,
			serializer,
			new ItemDigestor<TItem>(chf, serializer),
			keyChecksum,
			comparer,
			policy,
			reservedRecords,
			endianness
		) {
	}

	public TransactionalHashSet(string filename, string uncommittedPageFileDir, IItemSerializer<TItem> serializer, IItemHasher<TItem> hasher, IItemChecksummer<byte[]> keyChecksum = null, IEqualityComparer<TItem> comparer = null,
	                            int transactionalPageSize = HydrogenDefaults.TransactionalPageSize, long maxMemory = HydrogenDefaults.MaxMemoryPerCollection, int clusterSize = HydrogenDefaults.ClusterSize,
	                            ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = HydrogenDefaults.Endianness, bool readOnly = false)
		: this(
			  new TransactionalStream(filename, uncommittedPageFileDir, transactionalPageSize, maxMemory, readOnly, readOnly),
			  clusterSize,
			  serializer,
			  hasher,
			  keyChecksum,
			  comparer,
			  policy,
			  reservedRecords,
			  endianness
		) {
	}

	public TransactionalHashSet(TransactionalStream transactionalStream, int clusterSize, IItemSerializer<TItem> serializer, IItemHasher<TItem> hasher, IItemChecksummer<byte[]> keyChecksum = null, IEqualityComparer<TItem> comparer = null,
								ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = HydrogenDefaults.Endianness)
		: this(
			new TransactionalDictionarySK<byte[], TItem>(
				transactionalStream,
				new StaticSizeByteArraySerializer(hasher.DigestLength),
				serializer,
				keyChecksum ?? new HashChecksummer(),
				new ByteArrayEqualityComparer(),
				comparer,
				clusterSize,
				policy | ClusteredStoragePolicy.TrackKey,
				reservedRecords,
				endianness
			),
			comparer,
			hasher
		) {
		Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in clustered dictionary implementations.");
	}

	public TransactionalHashSet(ITransactionalDictionary<byte[], TItem> internalTransactionalDictionary, IEqualityComparer<TItem> comparer, IItemHasher<TItem> hasher)
		: base(internalTransactionalDictionary, comparer, hasher) {
		_internalTransactionalDictionary = internalTransactionalDictionary;
	}

	public void Commit() => _internalTransactionalDictionary.Commit();

	public Task CommitAsync() => _internalTransactionalDictionary.CommitAsync();

	public void Rollback() => _internalTransactionalDictionary.Rollback();

	public Task RollbackAsync() => _internalTransactionalDictionary.RollbackAsync();

	public void Dispose() => _internalTransactionalDictionary.Dispose();
}
