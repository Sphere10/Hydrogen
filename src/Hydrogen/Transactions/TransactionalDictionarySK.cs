//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Threading.Tasks;

//namespace Hydrogen;

//public class TransactionalDictionaryCLK<TKey, TValue> : StreamMappedDictionaryCLK<TKey, TValue>, ITransactionalDictionary<TKey, TValue> {

//	public event EventHandlerEx<object> Committing { add => TransactionalStream.Committing += value; remove => TransactionalStream.Committing -= value; }
//	public event EventHandlerEx<object> Committed { add => TransactionalStream.Committed += value; remove => TransactionalStream.Committed -= value; }
//	public event EventHandlerEx<object> RollingBack { add => TransactionalStream.RollingBack += value; remove => TransactionalStream.RollingBack -= value; }
//	public event EventHandlerEx<object> RolledBack { add => TransactionalStream.RolledBack += value; remove => TransactionalStream.RolledBack -= value; }

//	public TransactionalDictionaryCLK(
//		string filename,
//		string uncommittedPageFileDir,
//		IItemSerializer<TKey> constantLengthKeySerializer,
//		IItemSerializer<TValue> valueSerializer,
//		IItemChecksummer<TKey> keyChecksum = null,
//		IEqualityComparer<TKey> keyComparer = null,
//		IEqualityComparer<TValue> valueComparer = null,
//		int transactionalPageSize = HydrogenDefaults.TransactionalPageSize,
//		long maxMemory = HydrogenDefaults.MaxMemoryPerCollection,
//		int clusterSize = HydrogenDefaults.ClusterSize,
//		StreamContainerPolicy policy = StreamContainerPolicy.Default,
//		Endianness endianness = HydrogenDefaults.Endianness,
//		bool readOnly = false,
//		bool autoLoad = false
//	) : this(
//		new TransactionalStream(
//			filename, 
//			uncommittedPageFileDir, 
//			transactionalPageSize, 
//			maxMemory, 
//			readOnly, 
//			autoLoad
//		),
//		clusterSize,
//		constantLengthKeySerializer,
//		valueSerializer,
//		keyChecksum,
//		keyComparer,
//		valueComparer,
//		policy,
//		endianness,
//		autoLoad
//	) {
//	}

//	public TransactionalDictionaryCLK(
//		TransactionalStream transactionalStream,
//		int clusterSize,
//		IItemSerializer<TKey> constantLengthKeySerializer,
//		IItemSerializer<TValue> valueSerializer = null,
//		IItemChecksummer<TKey> keyChecksum = null,
//		IEqualityComparer<TKey> keyComparer = null,
//		IEqualityComparer<TValue> valueComparer = null,
//		StreamContainerPolicy policy = StreamContainerPolicy.Default,
//		Endianness endianness = Endianness.LittleEndian,
//		bool autoLoad = false
//	) : base(
//		transactionalStream,
//		clusterSize,
//		constantLengthKeySerializer,
//		valueSerializer,
//		keyComparer,
//		valueComparer,
//		policy,
//		endianness,
//		autoLoad) {
//	}

//	protected TransactionalStream TransactionalStream => (TransactionalStream)ObjectContainer.StreamContainer.RootStream;
	
//	public void Commit() => TransactionalStream.Commit();

//	public Task CommitAsync() => TransactionalStream.CommitAsync();

//	public void Rollback() => TransactionalStream.Rollback();

//	public Task RollbackAsync() => TransactionalStream.RollbackAsync();

//	public void Dispose() => TransactionalStream.Dispose();

//}
