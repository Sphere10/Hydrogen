// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Base implementation for an <see cref="IMetaDataStore{TData}"/>.
/// </summary>
/// <typeparam name="TData">The type of meta-data being stored</typeparam>
internal abstract class MetaDataStoreBase<TData> : ClusteredStreamsAttachmentBase, IMetaDataStore<TData> {

	protected MetaDataStoreBase(ClusteredStreams streams, long reservedStreamIndex)
		: base(streams, reservedStreamIndex) {
	}

	public abstract long Count { get; }

	public abstract TData Read(long index);

	public abstract byte[] ReadBytes(long index);

	public abstract void Add(long index, TData data);

	public abstract void Update(long index, TData data);

	public abstract void Insert(long index, TData data);

	public abstract void Remove(long index);

	public abstract void Reap(long index);

	public abstract void Clear();


}
