// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public static class TypeChanger {

	public static object SanitizeObject(object obj) {
		switch (Type.GetTypeCode(obj.GetType())) {
			case TypeCode.SByte:
				return (long)(SByte)obj;
			case TypeCode.Byte:
				return (long)(Byte)obj;
			case TypeCode.Int16:
				return (long)(Int16)obj;
			case TypeCode.UInt16:
				return (long)(UInt16)obj;
			case TypeCode.Int32:
				return (long)(Int32)obj;
			case TypeCode.UInt32:
				return (long)(UInt32)obj;
			case TypeCode.Int64:
				return (long)(Int64)obj;
			case TypeCode.UInt64:
				return (long)(UInt64)obj;
			case TypeCode.Single:
				return (double)(Single)obj;
			case TypeCode.Double:
				return (double)(Double)obj;
			case TypeCode.Decimal:
				return obj;
			case TypeCode.Empty:
			case TypeCode.Object:
			case TypeCode.DBNull:
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.DateTime:
			case TypeCode.String:
			default:
				return obj;
		}
	}

	public static object ChangeType(object value, Type targetType) {
		var isNullable = targetType.IsNullable();
		var isReferenceType = !targetType.IsValueType;
		if ((isNullable || isReferenceType) && (value == null || value == DBNull.Value))
			return null;
		if (value == null)
			throw new InvalidOperationException("Unable to convert NULL to a value type");

		if (targetType.IsInstanceOfType(value))
			return value;
		return Convert.ChangeType(value, isNullable ? Nullable.GetUnderlyingType(targetType) : targetType);
	}

	public static T ChangeType<T>(object value)
		=> (T)ChangeType(value, typeof(T));

}
