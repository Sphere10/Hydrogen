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