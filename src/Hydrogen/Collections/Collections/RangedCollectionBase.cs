// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// A base class for batch-optimized extended collections. Much of the boiler plate code is provided, implementations only
/// care about important methods.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class RangedCollectionBase<T> : ExtendedCollectionBase<T> {

	public override bool IsReadOnly => false;


	public sealed override bool Contains(T item) {
		return ContainsRange(new[] { item }).First();
	}

	public sealed override void Add(T item) {
		AddRange(new[] { item });
	}

	public sealed override bool Remove(T item) {
		return RemoveRange(new[] { item }).First();
	}

	public override void CopyTo(T[] array, int arrayIndex) {
		foreach (var item in this)
			array[arrayIndex++] = item;
	}

	protected void CheckNotReadonly() {
		if (IsReadOnly)
			throw new InvalidOperationException("Collection is read-only");
	}

}
