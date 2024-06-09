// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hydrogen;

internal class StreamLockingList<TData> : ExtendedListDecorator<TData> {
	private readonly IClusteredStreamsAttachment _attachment;

	public StreamLockingList(IExtendedList<TData> innerList, IClusteredStreamsAttachment attachment)
		: base(innerList) {
		_attachment = attachment;
	}

	public override bool IsReadOnly {
		get {
			CheckAttached();
			using var _ = _attachment.Streams.EnterAccessScope();
			return base.IsReadOnly;
		}
	}

	public override long Count {
		get {
			CheckAttached();
			using var _ = _attachment.Streams.EnterAccessScope();
			return base.Count;
		}
	}

	public override IEnumerable<TData> ReadRange(long index, long count) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		return base.ReadRange(index, count);
	}

	public override TData Read(long index) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		return base.Read(index);
	}

	public override IEnumerable<bool> ContainsRange(IEnumerable<TData> items) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		return base.ContainsRange(items);
	}

	public override bool Contains(TData item) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		return base.Contains(item);
	}

	public override IEnumerable<long> IndexOfRange(IEnumerable<TData> items) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		return base.IndexOfRange(items);
	}

	public override long IndexOfL(TData item) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		return base.IndexOfL(item);
	}

	public override void AddRange(IEnumerable<TData> items) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		base.AddRange(items);
	}

	public override void Add(TData item) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		base.Add(item);
	}

	public override void InsertRange(long index, IEnumerable<TData> items) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		base.InsertRange(index, items);
	}

	public override void Insert(long index, TData item) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		base.Insert(index, item);
	}

	public override void UpdateRange(long index, IEnumerable<TData> items) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		base.UpdateRange(index, items);
	}

	public override void Update(long index, TData item) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		base.Update(index, item);
	}

	public override void RemoveRange(long index, long count) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		base.RemoveRange(index, count);
	}

	public override IEnumerable<bool> RemoveRange(IEnumerable<TData> items) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		return base.RemoveRange(items);
	}

	public override bool Remove(TData item) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		return base.Remove(item);
	}

	public override void RemoveAt(long index) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		base.RemoveAt(index);
	}

	public override void Clear() {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		base.Clear();
	}

	public override void CopyTo(TData[] array, int arrayIndex) {
		CheckAttached();
		using var _ = _attachment.Streams.EnterAccessScope();
		base.CopyTo(array, arrayIndex);
	}

	public override IEnumerator<TData> GetEnumerator() {
		CheckAttached();
		var scope = _attachment.Streams.EnterAccessScope();
		try {
			return
				base
					.GetEnumerator()
					.OnDispose(scope.Dispose);
		} catch {
			scope.Dispose();
			throw;
		}
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CheckAttached() {
		if (!_attachment.IsAttached)
			throw new InvalidOperationException("Index is not attached");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CheckNotAttached() {
		if (_attachment.IsAttached)
			throw new InvalidOperationException("Index is already attached");
	}
}
