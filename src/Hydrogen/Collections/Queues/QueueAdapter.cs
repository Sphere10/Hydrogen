using System;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Wraps a <see cref="Queue"/> as an <see cref="ICollection{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class QueueAdapter<T> : IQueue<T>, IReadOnlyCollection<T>  {
	private readonly Queue<T> _queue;
	
	public QueueAdapter() : this(new Queue<T>(), false) {
	}
	
	public QueueAdapter(Queue<T> queue) : this(queue, false) {
	}

	public QueueAdapter(Queue<T> queue, bool readOnly) {
		_queue = queue;
		IsReadOnly = readOnly;
	}

	public int Count => _queue.Count;

	public bool IsReadOnly { get; }

	public void Add(T item) => _queue.Enqueue(item);

	public void Clear() => _queue.Clear();

	public bool Contains(T item) => _queue.Contains(item);

	public void CopyTo(T[] array, int arrayIndex) => _queue.CopyTo(array, arrayIndex);

	public bool Remove(T item) =>  throw new NotSupportedException();

	public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();
	
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


	public bool TryPeek(out T value) { 
		if (_queue.Count < 1) {
			value = default;
			return false;
		}
		value = _queue.Peek();
		return true;
	}

	public bool TryDequeue(out T value) {
		if (_queue.Count < 1) {
			value = default;
			return false;
		}
		value = _queue.Dequeue();
		return true;		
	}

	public void Enqueue(T item)  => _queue.Enqueue(item);
}
