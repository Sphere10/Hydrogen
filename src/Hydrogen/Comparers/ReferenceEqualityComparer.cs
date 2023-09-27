﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hydrogen;

public sealed class ReferenceEqualityComparer : IEqualityComparer<object?>, IEqualityComparer {

	public ReferenceEqualityComparer() {
	}

	/// <summary>
	/// Gets the singleton <see cref="ReferenceEqualityComparer"/> instance.
	/// </summary>
	public static ReferenceEqualityComparer Instance { get; } = new();

	/// <summary>
	/// Determines whether two object references refer to the same object instance.
	/// </summary>
	/// <param name="x">The first object to compare.</param>
	/// <param name="y">The second object to compare.</param>
	/// <returns>
	/// <see langword="true"/> if both <paramref name="x"/> and <paramref name="y"/> refer to the same object instance
	/// or if both are <see langword="null"/>; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This API is a wrapper around <see cref="object.ReferenceEquals(object?, object?)"/>.
	/// It is not necessarily equivalent to calling <see cref="object.Equals(object?, object?)"/>.
	/// </remarks>
	public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

	/// <summary>
	/// Returns a hash code for the specified object. The returned hash code is based on the object
	/// identity, not on the contents of the object.
	/// </summary>
	/// <param name="obj">The object for which to retrieve the hash code.</param>
	/// <returns>A hash code for the identity of <paramref name="obj"/>.</returns>
	/// <remarks>
	/// This API is a wrapper around <see cref="RuntimeHelpers.GetHashCode(object)"/>.
	/// It is not necessarily equivalent to calling <see cref="object.GetHashCode()"/>.
	/// </remarks>
	public int GetHashCode(object? obj) {
		// Depending on target framework, RuntimeHelpers.GetHashCode might not be annotated
		// with the proper nullability attribute. We'll suppress any warning that might
		// result.
		return RuntimeHelpers.GetHashCode(obj!);
	}
}
