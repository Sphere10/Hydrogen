// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// When dealing with object containers, auxilliary components that attached to the container (such as <see cref="IndexBase{TItem,TKey}"/>
/// derived classes) need to participate in the lifecycle of the container. By deriving from this, they can be attached  to the container
/// via <see cref="ObjectContainer.RegisterAttachment"/>.
/// </summary>
public abstract class ObjectContainerAttachmentBase : IDisposable, IObjectContainerAttachment {
	private Stream _stream;
	private readonly long _streamOffset;
	private bool _attached;

	protected ObjectContainerAttachmentBase(ObjectContainer objectContainer, long reservedStreamIndex) {
		Container = objectContainer;
		ReservedStreamIndex = reservedStreamIndex;
		_attached = false;
		_streamOffset = 0;
	}

	public ObjectContainer Container { get; }

	public long ReservedStreamIndex { get; }

	protected Stream Stream {
		get {
			CheckAttached();
			return _stream;
		}
		private set => _stream = value;
	}

	public bool IsAttached => _attached;

	public void Attach() {
		CheckNotAttached();
		Guard.Ensure(!Container.RequiresLoad, "Unable to attach to an unloaded Object Container");
		Guard.Ensure(Container.StreamContainer.Header.ReservedStreams > 0, "Stream Container has no reserved streams available");
		Guard.Ensure(ReservedStreamIndex < Container.StreamContainer.Header.ReservedStreams, $"Stream at index {ReservedStreamIndex} is not reserved");
		using (Container.StreamContainer.EnterAccessScope()) {
			_attached = true;

			// Open the stream used by the index. No access scope is acquired for the stream
			// and thus all use of the index must take place within an explicit access scope.
			Stream =
				Container
					.StreamContainer
					.Open(ReservedStreamIndex, false, false)
					.AsBounded(_streamOffset, long.MaxValue, allowInnerResize: true);

			// Ensures the stream is at least as long as the offset (the space prior to offset can
			// be used to store header information (i.e. factory info to decide what type of index to load)
			if (Stream.Position < 0)
				Stream.SetLength(0);

			AttachInternal();
		}
	}

	protected abstract void AttachInternal();

	public void Detach() {
		CheckAttached();
		DetachInternal();
		Stream.Dispose();
		Stream = null;
		_attached = false;
	}

	protected abstract void DetachInternal();

	public virtual void Dispose() {
		if (IsAttached)
			Detach();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CheckAttached() {
		if (!_attached)
			throw new InvalidOperationException("Index is not attached");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CheckNotAttached() {
		if (_attached)
			throw new InvalidOperationException("Index is already attached");
	}


}
