// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// Base implementation for an index on an <see cref="ObjectStream{T}"/>.
/// </summary>
public abstract class IndexBase<TStore> : ObjectStreamObserverBase, IClusteredStreamsAttachment where TStore : IClusteredStreamsAttachment {

	protected IndexBase(ObjectStream objectStream, TStore store)
		: base(objectStream) {
		Guard.ArgumentNotNull(store, nameof(store));
		Store = store;
	}

	public string AttachmentID => Store.AttachmentID;

	public TStore Store { get; }

	public virtual ClusteredStreams Streams => Store.Streams;

	public virtual bool IsAttached => Store.IsAttached;

	public virtual void Attach() => Store.Attach();

	public virtual void Detach() => Store.Detach();

	public virtual void Flush() => Store.Flush();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CheckAttached()
		=> Guard.Ensure(Store.IsAttached, "Index is not attached");

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CheckDetached()
		=> Guard.Ensure(!Store.IsAttached, "Index is attached");

}
