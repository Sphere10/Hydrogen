// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Hydrogen.FastReflection;

public class FieldAccessor {
	private Func<object, object> _getter;

	public FieldInfo FieldInfo { get; private set; }

	public FieldAccessor(FieldInfo fieldInfo) {
		this.FieldInfo = fieldInfo;
		this.Initialize(fieldInfo);

	}

	private void Initialize(FieldInfo fieldInfo) {
		// target: (object)(((TInstance)instance).Field)

		// preparing parameter, object type
		var instance = Expression.Parameter(typeof(object), "instance");

		// non-instance for static method, or ((TInstance)instance)
		var instanceCast = fieldInfo.IsStatic ? null : Expression.Convert(instance, fieldInfo.ReflectedType);

		// ((TInstance)instance).Property
		var fieldAccess = Expression.Field(instanceCast, fieldInfo);

		// (object)(((TInstance)instance).Property)
		var castFieldValue = Expression.Convert(fieldAccess, typeof(object));

		// Lambda expression
		var lambda = Expression.Lambda<Func<object, object>>(castFieldValue, instance);

		this._getter = lambda.Compile();
	}


	public object GetValue(object instance) {
		return this._getter(instance);
	}

}
