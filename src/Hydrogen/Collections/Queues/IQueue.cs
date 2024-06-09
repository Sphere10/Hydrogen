// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public interface IQueue<T> : ICollection<T> {
	public bool TryPeek(out T value);

	public bool TryDequeue(out T value);

	public void Enqueue(T item);
}


public static class IQueueExtensions {
	public static T Dequeue<T>(this IQueue<T> queue) {
		Guard.Ensure(queue.Count > 0, "Insufficient items");
		if (!queue.TryDequeue(out var value))
			throw new InvalidOperationException("Unable to dequeue from queue");
		return value;
	}

	public static T Peek<T>(this IQueue<T> queue) {
		Guard.Ensure(queue.Count > 1, "Insufficient items");
		if (!queue.TryPeek(out var value))
			throw new InvalidOperationException("Unable to peek from queue");
		return value;
	}
}
