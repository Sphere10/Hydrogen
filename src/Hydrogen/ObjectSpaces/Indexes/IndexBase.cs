// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.CompilerServices;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Base implementation for an index on an <see cref="ObjectStreamStream{T}"/>.
/// </summary>
/// <typeparam name="TItem">Type of item being stored in <see cref="ObjectStream{T}"/></typeparam>
/// <typeparam name="TKey">Type of property in <see cref="TItem"/> that is the key</typeparam>
public abstract class IndexBase<TData, TStore> : ContainerObserverBase, IClusteredStreamsAttachment where TStore : IMetaDataStore<TData> {

	protected IndexBase(ObjectStream objectStream, TStore keyStore)
		: base(objectStream) {
		Guard.ArgumentNotNull(objectStream, nameof(objectStream));
		KeyStore = keyStore;
	}

	public TStore KeyStore { get; }

	protected override void OnRemoved(long index) {
		CheckAttached();
		KeyStore.Remove(index);
	}

	protected override void OnReaped(long index) {
		CheckAttached();
		KeyStore.Reap(index);
	}

	protected override void OnContainerClearing() {
		// When the objectStream about to be cleared, we detach the observer
		CheckAttached();
		KeyStore.Detach();
	}

	protected override void OnContainerCleared() {
		// After objectStream was cleared, we reboot the index
		CheckDetached();
		KeyStore.Attach();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CheckAttached()
		=> Guard.Ensure(KeyStore.IsAttached, "Index is not attached");

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CheckDetached()
		=> Guard.Ensure(!KeyStore.IsAttached, "Index is attached");

	#region IObjectContainerAttachment Implementation

	// NOTE: use of backing field _keyStore to avoid attached check

	ClusteredStreams IClusteredStreamsAttachment.Streams => KeyStore.Streams;

	long IClusteredStreamsAttachment.ReservedStreamIndex => KeyStore.ReservedStreamIndex; 

	bool IClusteredStreamsAttachment.IsAttached => KeyStore.IsAttached;

	void IClusteredStreamsAttachment.Attach() => KeyStore.Attach();

	void IClusteredStreamsAttachment.Detach() => KeyStore.Detach();

	#endregion
}
