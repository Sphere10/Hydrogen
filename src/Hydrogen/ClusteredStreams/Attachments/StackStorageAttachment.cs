// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public class StackStorageAttachment<TData> : PagedListStorageAttachmentBase<TData>, IStack<TData> {

	private IStack<TData> _stackImpl;

	public StackStorageAttachment(ClusteredStreams streams, string attachmentID, IItemSerializer<TData> datumSerializer)
		: base(streams, attachmentID, datumSerializer) {
	}

	public long Count {
		get {
			CheckAttached();
			using var _ = Streams.EnterAccessScope();
			return _stackImpl.Count;
		}
	}

	int ICollection<TData>.Count => checked((int)Count);

	public bool IsReadOnly {
		get {
			CheckAttached();
			return _stackImpl.IsReadOnly;
		}
	}

	public void Push(TData item) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		_stackImpl.Push(item);
	}

	public bool TryPeek(out TData value) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return _stackImpl.TryPeek(out value);
	}

	public bool TryPop(out TData value) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return _stackImpl.TryPop(out value);
	}

	public void Clear() {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		_stackImpl.Clear();
	}

	public bool Remove(TData item) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return _stackImpl.Remove(item);
	}

	public override int GetHashCode() {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return _stackImpl.GetHashCode();
	}

	public void Add(TData item) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		_stackImpl.Add(item);
	}

	public bool Contains(TData item) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return _stackImpl.Contains(item);
	}

	public void CopyTo(TData[] array, int arrayIndex) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		_stackImpl.CopyTo(array, arrayIndex);
	}

	public IEnumerator<TData> GetEnumerator() {
		CheckAttached();
		var scope = Streams.EnterAccessScope();
		try {
			return 
				_stackImpl
				.GetEnumerator()
				.OnDispose(scope.Dispose);
		} catch {
			scope.Dispose();
			throw;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	protected override void AttachInternal() {
		base.AttachInternal();
		_stackImpl = new StackAdapter<TData>(PagedList);
	}

	protected override void DetachInternal() {
		base.DetachInternal();
		_stackImpl = null;
	}

}
