// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Reflection;
using Hydrogen.FastReflection;


namespace Tools;

public static class Reflection {

	public static T As<T>(object obj) {
		return Tools.Object.ChangeType<T>(obj);
	}

	public static object GetPropertyValue(object obj, string propertyName) {
		return obj.GetType().GetProperty(propertyName)?.FastGetValue(obj);
	}

	public static T GetPropertyValue<T>(object obj, string propertyName) {
		var val = GetPropertyValue(obj, propertyName);
		return val == null ? default(T) : Tools.Object.ChangeType<T>(val);
	}

	public static void SetPropertyValue(object obj, string propertyName, object value) {
		obj.GetType().GetProperty(propertyName)?.FastSetValue(obj, value);
	}

	public static object GetFieldValue(object obj, string propertyName) {
		return obj.GetType().GetField(propertyName)?.FastGetValue(obj);
	}

	public static T GetFieldValue<T>(object obj, string propertyName) {
		var val = GetFieldValue(obj, propertyName);
		return val == null ? default(T) : Tools.Object.ChangeType<T>(val);
	}

	public static void SetFieldValue(object obj, string propertyName, object value) {
		obj.GetType().GetField(propertyName).SetValue(obj, value);
	}



	///// <summary>
	///// Returns a _private_ Property Value from a given Object. Uses Reflection.
	///// Throws a ArgumentOutOfRangeException if the Property is not found.
	///// </summary>
	///// <typeparam name="T">Type of the Property</typeparam>
	///// <param name="obj">Object from where the Property Value is returned</param>
	///// <param name="propName">Propertyname as string.</param>
	///// <returns>PropertyValue</returns>
	//public static T GetPrivatePropertyValue<T>(object obj, string propName) {
	//	if (obj == null) throw new ArgumentNullException("obj");
	//	PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	//	if (pi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
	//	return Tools.Object.ChangeType<T>(pi.GetValue(obj, null));
	//}

	///// <summary>
	///// Returns a private Property Value from a given Object. Uses Reflection.
	///// Throws a ArgumentOutOfRangeException if the Property is not found.
	///// </summary>
	///// <typeparam name="T">Type of the Property</typeparam>
	///// <param name="obj">Object from where the Property Value is returned</param>
	///// <param name="propName">Propertyname as string.</param>
	///// <returns>PropertyValue</returns>
	//public static T GetPrivateFieldValue<T>(object obj, string propName) {
	//	if (obj == null) throw new ArgumentNullException("obj");
	//	Type t = obj.GetType();
	//	FieldInfo fi = null;
	//	while (fi == null && t != null) {
	//		fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	//		t = t.BaseType;
	//	}
	//	if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
	//	return Tools.Object.ChangeType<T>(fi.GetValue(obj));
	//}

	///// <summary>
	///// Sets a _private_ Property Value from a given Object. Uses Reflection.
	///// Throws a ArgumentOutOfRangeException if the Property is not found.
	///// </summary>
	///// <typeparam name="T">Type of the Property</typeparam>
	///// <param name="obj">Object from where the Property Value is set</param>
	///// <param name="propName">Propertyname as string.</param>
	///// <param name="val">Value to set.</param>
	///// <returns>PropertyValue</returns>
	//public static void SetPrivatePropertyValue<T>(object obj, string propName, T val) {
	//	Type t = obj.GetType();
	//	if (t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
	//		throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
	//	t.InvokeMember(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, obj, new object[] { val });
	//}

	///// <summary>
	///// Set a private Property Value on a given Object. Uses Reflection.
	///// </summary>
	///// <typeparam name="T">Type of the Property</typeparam>
	///// <param name="obj">Object from where the Property Value is returned</param>
	///// <param name="propName">Propertyname as string.</param>
	///// <param name="val">the value to set</param>
	///// <exception cref="ArgumentOutOfRangeException">if the Property is not found</exception>
	//public static void SetPrivateFieldValue<T>(object obj, string propName, T val) {
	//	if (obj == null) throw new ArgumentNullException("obj");
	//	Type t = obj.GetType();
	//	FieldInfo fi = null;
	//	while (fi == null && t != null) {
	//		fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	//		t = t.BaseType;
	//	}
	//	if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
	//	fi.SetValue(obj, val);
	//}

	public static object InvokeMethod(object obj, string methodName, params object[] parameters) {
		return
			obj
				.GetType()
				.GetMethod(
					methodName,
					BindingFlags.Instance |
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.Static |
					BindingFlags.FlattenHierarchy
				)
				.FastInvoke(obj, parameters);
	}

	public static T InvokeMethod<T>(object obj, string methodName, params object[] parameters) {
		var val = InvokeMethod(obj, methodName, parameters);
		return val == null ? default(T) : Tools.Object.ChangeType<T>(val);
	}

}
