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

/// <summary>
/// Wraps a <see cref="Queue"/> as an <see cref="ICollection{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class QueueListAdapter<T> : IQueue<T> {
	private readonly IList<T> _queue;
	public QueueListAdapter(IList<T> queue) : this(queue, false) {
	}

	public QueueListAdapter(IList<T> queue, bool readOnly) {
		_queue = queue;
		IsReadOnly = readOnly;
	}

	public int Count => _queue.Count;

	public bool IsReadOnly { get; }

	public void Add(T item) => _queue.Add(item);

	public void Clear() => _queue.Clear();

	public bool Contains(T item) => _queue.Contains(item);

	public void CopyTo(T[] array, int arrayIndex) => _queue.CopyTo(array, arrayIndex);

	public bool Remove(T item) => throw new NotSupportedException();

	public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public bool TryPeek(out T value) {
		if (_queue.Count < 1) {
			value = default;
			return false;
		}
		value = _queue[^1];
		_queue.RemoveAt(^1);
		return true;
	}

	public bool TryDequeue(out T value) {
		if (_queue.Count < 1) {
			value = default;
			return false;
		}
		value = _queue[^1];
		return true;
	}

	public void Enqueue(T item) => _queue.Add(item);
}
