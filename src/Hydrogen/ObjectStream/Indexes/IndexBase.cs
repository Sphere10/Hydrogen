// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// Base implementation for an index on an <see cref="ObjectStream{T}"/>.
/// </summary>
public abstract class IndexBase<TData, TStore> : ObjectStreamObserverBase, IClusteredStreamsAttachment where TStore : IMetaDataStore<TData> {

	protected IndexBase(ObjectStream objectStream, TStore keyStore)
		: base(objectStream) {
		Guard.ArgumentNotNull(objectStream, nameof(objectStream));
		KeyStore = keyStore;
	}

	public TStore KeyStore { get; }

	public virtual ClusteredStreams Streams => KeyStore.Streams;

	public virtual long ReservedStreamIndex => KeyStore.ReservedStreamIndex; 

	public virtual bool IsAttached => KeyStore.IsAttached;

	public virtual void Attach() => KeyStore.Attach();

	public virtual void Detach() => KeyStore.Detach();

	public virtual void Flush() => KeyStore.Flush();

	protected override void OnRemoved(long index) {
		CheckAttached();
		KeyStore.Remove(index);
	}

	protected override void OnReaped(long index) {
		CheckAttached();
		KeyStore.Reap(index);
	}

	protected override void OnContainerClearing() {
		// Inform the key store to clear
		KeyStore.Clear();

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

}
