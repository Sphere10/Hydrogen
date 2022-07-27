using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Linq;

namespace Hydrogen;

public class StackList<T> : ExtendedListDecorator<T>, IStack<T> {

	public StackList() : this(new ExtendedList<T>()) {
	}

	public StackList(IExtendedList<T> internalExtendedList) : base(internalExtendedList) {
	}
	
	public bool TryPeek(out T value) => TryPeek(out value, 1);

	public bool TryPeek(out T value, int depth) {
		var count = Count;
		var itemIX = count - depth;
		if (itemIX < 0) {
			value = default(T);
			return false;
		}
		value = base.Read(itemIX);
		return true;
	}


	public bool TryPop(out T value) {
		var count = Count;
		if (count <= 0) {
			value = default(T);
			return false;
		}
		value = base.Read(count - 1);
		base.RemoveAt(count - 1);
		return true;
	}

	
	public T Peek(int depth) {
		if (!TryPeek(out var value, depth))
			throw new InvalidOperationException($"Unable to peek stack at depth {depth}");
		return value;
	}

	public void Push(T item) => base.Add(item);
}

