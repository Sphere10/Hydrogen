using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen;

public class TransactionalDictionarySK<TKey, TValue> : StreamMappedDictionarySK<TKey, TValue>, ITransactionalDictionary<TKey, TValue> {

	public event EventHandlerEx<object> Committing { add => _valueStore.Committing += value; remove => _valueStore.Committing -= value; }
	public event EventHandlerEx<object> Committed { add => _valueStore.Committed += value; remove => _valueStore.Committed -= value; }
	public event EventHandlerEx<object> RollingBack { add => _valueStore.RollingBack += value; remove => _valueStore.RollingBack -= value; }
	public event EventHandlerEx<object> RolledBack { add => _valueStore.RolledBack += value; remove => _valueStore.RolledBack -= value; }

	private readonly ITransactionalList<TValue> _valueStore;

	public TransactionalDictionarySK(string filename, string uncommittedPageFileDir, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IItemChecksummer<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null,
	                                 IEqualityComparer<TValue> valueComparer = null, int transactionalPageSize = HydrogenDefaults.TransactionalPageSize, long maxMemory = HydrogenDefaults.MaxMemoryPerCollection, int clusterSize = HydrogenDefaults.ClusterSize,
	                                 StreamContainerPolicy policy = StreamContainerPolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = HydrogenDefaults.Endianness, bool readOnly = false, bool autoLoad = false)
		: this(
			new TransactionalStream(filename, uncommittedPageFileDir, transactionalPageSize, maxMemory, readOnly, autoLoad),
			keySerializer,
			valueSerializer,
			keyChecksum,
			keyComparer,
			valueComparer,
			clusterSize,
			policy,
			reservedRecords,
			endianness
		) {
	}

	public TransactionalDictionarySK(TransactionalStream transactionalStream, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IItemChecksummer<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null,
	                                 IEqualityComparer<TValue> valueComparer = null, int clusterSize = HydrogenDefaults.ClusterSize, StreamContainerPolicy policy = StreamContainerPolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = HydrogenDefaults.Endianness)
		: this(
			new TransactionalList<TValue>(
				transactionalStream,
				valueSerializer,
				valueComparer,
				clusterSize, policy, reservedRecords, keySerializer.StaticSize, endianness
			),
			keySerializer,
			valueSerializer,
			keyChecksum,
			keyComparer,
			valueComparer,
			endianness
		) {
	}

	public TransactionalDictionarySK(ITransactionalList<TValue> valueStore, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IItemChecksummer<TKey> keyChecksummer = null, IEqualityComparer<TKey> keyComparer = null,
									 IEqualityComparer<TValue> valueComparer = null, Endianness endianness = Endianness.LittleEndian)
		: base(valueStore, keySerializer, valueSerializer, keyChecksummer, keyComparer, valueComparer, endianness) {
		Guard.ArgumentNotNull(valueStore, nameof(valueStore));
		Guard.ArgumentNotNull(keySerializer, nameof(keySerializer));
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		_valueStore = valueStore;
	}

	public void Commit() => _valueStore.Commit();

	public Task CommitAsync() => _valueStore.CommitAsync();

	public void Rollback() => _valueStore.Rollback();

	public Task RollbackAsync() => _valueStore.RollbackAsync();

	public void Dispose() => _valueStore.Dispose();
}
