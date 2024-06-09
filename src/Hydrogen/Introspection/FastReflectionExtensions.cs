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

public static class FastReflectionExtensions {
	public static object FastInvoke(this MethodInfo methodInfo, object instance, params object[] parameters) {
		return FastReflectionCaches.MethodInvokerCache.Get(methodInfo).Value.Invoke(instance, parameters);
	}

	public static void FastSetValue(this PropertyInfo propertyInfo, object instance, object value) {
		FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).Value.SetValue(instance, value);
	}

	public static object FastGetValue(this PropertyInfo propertyInfo, object instance) {
		return FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).Value.GetValue(instance);
	}

	public static object FastGetValue(this FieldInfo fieldInfo, object instance) {
		return FastReflectionCaches.FieldAccessorCache.Get(fieldInfo).Value.GetValue(instance);
	}

	public static object FastInvoke(this ConstructorInfo constructorInfo, params object[] parameters) {
		return FastReflectionCaches.ConstructorInvokerCache.Get(constructorInfo).Value.Invoke(parameters);
	}

	public static string[] FastGetEnumNames(this Type type) {
		return FastReflectionCaches.EnumNamesCache[type];
	}


	public static bool FastIsSubTypeOf(this Type type, Type other) {
		return FastReflectionCaches.IsSubTypeCache[(type, other)];
	}
}
