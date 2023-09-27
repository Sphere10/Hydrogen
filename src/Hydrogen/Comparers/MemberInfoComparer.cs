// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hydrogen.FastReflection;

/// <summary>
/// Compares <see cref="MemberInfo"/> and ensures their owner types are also equal (.NET default does not do this).
/// </summary>
/// <typeparam name="T"></typeparam>
public class MemberInfoComparer<T> : IEqualityComparer<T> where T : MemberInfo {
	public bool Equals(T x, T y) => (x == null && y == null) || x.ReflectedType != null && x.Equals(y) && x.ReflectedType == y.ReflectedType;

	public int GetHashCode(T obj) => HashCode.Combine(obj.GetHashCode(), obj.ReflectedType.GetHashCode());

}
