// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Reflection;

namespace Hydrogen.FastReflection;

public static class FastReflectionCaches {
	static FastReflectionCaches() {
		EnumNamesCache = new ActionCache<Type, string[]>(t => t.GetEnumNames());
		MethodInvokerCache = new ActionCache<MethodInfo, MethodInvoker>(mi => new MethodInvoker(mi), keyComparer: new MemberInfoComparer<MethodInfo>());
		PropertyAccessorCache = new ActionCache<PropertyInfo, PropertyAccessor>(pi => new PropertyAccessor(pi), keyComparer: new MemberInfoComparer<PropertyInfo>());
		FieldAccessorCache = new ActionCache<FieldInfo, FieldAccessor>(fi => new FieldAccessor(fi), keyComparer: new MemberInfoComparer<FieldInfo>());
		ConstructorInvokerCache = new ActionCache<ConstructorInfo, ConstructorInvoker>(ci => new ConstructorInvoker(ci), keyComparer: new MemberInfoComparer<ConstructorInfo>());
		IsSubTypeCache = new ActionCache<(Type, Type), bool>(t => t.Item1.IsSubTypeOf(t.Item2));
	}

	public static ICache<Type, string[]> EnumNamesCache { get; set; }

	public static ICache<MethodInfo, MethodInvoker> MethodInvokerCache { get; set; }

	public static ICache<PropertyInfo, PropertyAccessor> PropertyAccessorCache { get; set; }

	public static ICache<FieldInfo, FieldAccessor> FieldAccessorCache { get; set; }

	public static ICache<ConstructorInfo, ConstructorInvoker> ConstructorInvokerCache { get; set; }

	public static ICache<(Type, Type), bool> IsSubTypeCache { get; set; }

}
