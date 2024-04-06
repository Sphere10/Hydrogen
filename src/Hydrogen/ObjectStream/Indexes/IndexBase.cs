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

	protected IndexBase(ObjectStream objectStream, TStore store)
		: base(objectStream) {
		Guard.ArgumentNotNull(objectStream, nameof(objectStream));
		Store = store;
	}

	public TStore Store { get; }

	public virtual ClusteredStreams Streams => Store.Streams;

	public virtual long ReservedStreamIndex => Store.ReservedStreamIndex; 

	public virtual bool IsAttached => Store.IsAttached;

	public virtual void Attach() => Store.Attach();

	public virtual void Detach() => Store.Detach();

	public virtual void Flush() => Store.Flush();

	protected override void OnRemoved(long index) {
		CheckAttached();
		Store.Remove(index);
	}

	protected override void OnReaped(long index) {
		CheckAttached();
		Store.Reap(index);
	}

	protected override void OnContainerClearing() {
		// Inform the key store to clear
		Store.Clear();

		// When the objectStream about to be cleared, we detach the observer
		CheckAttached();
		Store.Detach();
	}

	protected override void OnContainerCleared() {
		// After objectStream was cleared, we reboot the index
		CheckDetached();
		Store.Attach();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CheckAttached()
		=> Guard.Ensure(Store.IsAttached, "Index is not attached");

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CheckDetached()
		=> Guard.Ensure(!Store.IsAttached, "Index is attached");

}
