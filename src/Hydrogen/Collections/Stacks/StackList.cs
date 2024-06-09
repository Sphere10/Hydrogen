// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class StackList<T, TInner> : ExtendedListDecorator<T, TInner>, IStack<T> where TInner : IExtendedList<T> {
	
	public StackList(TInner internalExtendedList) : base(internalExtendedList) {
	}

	public bool TryPeek(out T value) => TryPeek(out value, 1);

	public virtual bool TryPeek(out T value, int depth) {
		var count = Count;
		var itemIX = count - depth;
		if (itemIX < 0) {
			value = default(T);
			return false;
		}
		value = base.Read(itemIX);
		return true;
	}

	public virtual bool TryPop(out T value) {
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

	public virtual void Push(T item) => base.Add(item);
}

public class StackList<T> : StackList<T, IExtendedList<T>> {

	public StackList() : this(new ExtendedList<T>()) {
	}

	public StackList(IExtendedList<T> list) : base(list) {
	}

}
