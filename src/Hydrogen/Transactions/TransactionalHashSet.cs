// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen;

public class TransactionalHashSet<TItem> : StreamMappedHashSet<TItem>, ITransactionalHashSet<TItem> {
	
	public event EventHandlerEx Committing { add => InternalDictionary.Committing += value; remove => InternalDictionary.Committing -= value; }
	public event EventHandlerEx Committed { add => InternalDictionary.Committed += value; remove => InternalDictionary.Committed -= value; }
	public event EventHandlerEx RollingBack { add => InternalDictionary.RollingBack += value; remove => InternalDictionary.RollingBack -= value; }
	public event EventHandlerEx RolledBack { add => InternalDictionary.RolledBack += value; remove => InternalDictionary.RolledBack -= value; }

	internal new readonly ITransactionalDictionary<byte[], TItem> InternalDictionary;

	public TransactionalHashSet(
		HydrogenFileDescriptor fileDescriptor,
		IItemSerializer<TItem> serializer,
		CHF chf = CHF.SHA2_256, 
		IItemChecksummer<byte[]> keyChecksum = null, 
		IEqualityComparer<TItem> comparer = null,
		int reservedStreamCount = 2,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string keyChecksumIndexName = HydrogenDefaults.DefaultKeyChecksumIndexName,
		FileAccessMode accessMode = FileAccessMode.Default
	) : this(
		new TransactionalStream(fileDescriptor, accessMode),
		fileDescriptor.ClusterSize,
		serializer,
		new ItemDigestor<TItem>(chf, serializer),
		keyChecksum,
		comparer,
		fileDescriptor.ContainerPolicy,
		reservedStreamCount,
		recyclableIndexIndexName,
		keyChecksumIndexName,
		fileDescriptor.Endianness,
		accessMode.IsReadOnly(),
		accessMode.HasFlag(FileAccessMode.AutoLoad)
	) {
	}

	public TransactionalHashSet(
		HydrogenFileDescriptor fileDescriptor,
		IItemSerializer<TItem> serializer, 
		IItemHasher<TItem> hasher, 
		IItemChecksummer<byte[]> keyChecksum = null, 
		IEqualityComparer<TItem> comparer = null,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default, 
		int reservedStreamCount = 2,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string keyChecksumIndexName = HydrogenDefaults.DefaultKeyChecksumIndexName,
		FileAccessMode accessMode = FileAccessMode.Default
	) : this(
		new TransactionalStream(fileDescriptor, accessMode),
		fileDescriptor.ClusterSize,
		serializer,
		hasher,
		keyChecksum,
		comparer,
		policy,
		reservedStreamCount,
		recyclableIndexIndexName,
		keyChecksumIndexName,
		fileDescriptor.Endianness,
		accessMode.IsReadOnly(),
		accessMode.HasFlag(FileAccessMode.AutoLoad)
	) {
	}

	public TransactionalHashSet(
		TransactionalStream transactionalStream,
		int clusterSize,
		IItemSerializer<TItem> serializer,
		IItemHasher<TItem> hasher,
		IItemChecksummer<byte[]> keyChecksum = null,
		IEqualityComparer<TItem> comparer = null,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default,
		int reservedStreamCount = 2,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string keyChecksumIndexName = HydrogenDefaults.DefaultKeyChecksumIndexName,
		Endianness endianness = HydrogenDefaults.Endianness,
		bool readOnly = false,
		bool autoLoad = false
	) : this(
		new TransactionalDictionary<byte[], TItem>(
			transactionalStream,
			new ConstantSizeByteArraySerializer(hasher.DigestLength),
			serializer,
			keyChecksum ?? new HashChecksummer(),
			ByteArrayEqualityComparer.Instance,
			comparer,
			clusterSize,
			policy,
			reservedStreamCount,
			recyclableIndexIndexName,
			keyChecksumIndexName,
			endianness,
			readOnly,
			autoLoad,
			StreamMappedDictionaryImplementation.ConstantLengthKeyBased
		),
		comparer,
		hasher
	) {
	}

	public TransactionalHashSet(
		ITransactionalDictionary<byte[], TItem> internalDictionary, 
		IEqualityComparer<TItem> comparer,
		IItemHasher<TItem> hasher
	) : base(internalDictionary, comparer, hasher) {
		InternalDictionary = internalDictionary;
	}

	public void Commit() => InternalDictionary.Commit();

	public Task CommitAsync() => InternalDictionary.CommitAsync();

	public void Rollback() => InternalDictionary.Rollback();

	public Task RollbackAsync() => InternalDictionary.RollbackAsync();

}
