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


public interface IStack<T> : ICollection<T> {

	new long Count { get; }

	public bool TryPeek(out T value);

	public bool TryPop(out T value);

	public void Push(T item);

}


public static class IStackExtensions {
	public static T Pop<T>(this IStack<T> stack) {
		Guard.Ensure(stack.Count > 0, "Stack is empty");
		if (!stack.TryPop(out var value))
			throw new InvalidOperationException("Unable to pop from stack");
		return value;
	}

	public static T Peek<T>(this IStack<T> stack) {
		Guard.Ensure(stack.Count > 1, "Stack is empty");
		if (!stack.TryPeek(out var value))
			throw new InvalidOperationException("Unable to pop from stack");
		return value;
	}
}
