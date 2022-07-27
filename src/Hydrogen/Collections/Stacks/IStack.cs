using System;
using System.Collections.Generic;

namespace Hydrogen;

public interface IStack<T> : ICollection<T> {

	public bool TryPeek(out T value);

	public bool TryPop(out T value);

	public void Push(T item);

}


public static class IStackExtensions {
	public static T Pop<T>(this IStack<T> stack) {
		Guard.Ensure(stack.Count > 0, "Insufficient items");
		if (!stack.TryPop(out var value))
			throw new InvalidOperationException("Unable to pop from stack");
		return value;
	}

	public static T Peek<T>(this IStack<T> stack) {
		Guard.Ensure(stack.Count > 1, "Insufficient items");
		if (!stack.TryPeek(out var value))
			throw new InvalidOperationException("Unable to pop from stack");
		return value;
	}
}
