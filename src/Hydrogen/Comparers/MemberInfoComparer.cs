// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
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
