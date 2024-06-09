// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Hydrogen.FastReflection;

public class PropertyAccessor {
	private Func<object, object> _mGetter;
	private MethodInvoker _mSetMethodInvoker;

	public PropertyInfo PropertyInfo { get; private set; }

	public PropertyAccessor(PropertyInfo propertyInfo) {
		this.PropertyInfo = propertyInfo;
		this.InitializeGet(propertyInfo);
		this.InitializeSet(propertyInfo);
	}

	private void InitializeGet(PropertyInfo propertyInfo) {
		if (!propertyInfo.CanRead) return;

		// Target: (object)(((TInstance)instance).Property)

		// preparing parameter, object type
		var instance = Expression.Parameter(typeof(object), "instance");

		// non-instance for static method, or ((TInstance)instance)
		var instanceCast = propertyInfo.GetGetMethod(true).IsStatic ? null : Expression.Convert(instance, propertyInfo.ReflectedType);

		// ((TInstance)instance).Property
		var propertyAccess = Expression.Property(instanceCast, propertyInfo);

		// (object)(((TInstance)instance).Property)
		var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));

		// Lambda expression
		var lambda = Expression.Lambda<Func<object, object>>(castPropertyValue, instance);

		this._mGetter = lambda.Compile();
	}

	private void InitializeSet(PropertyInfo propertyInfo) {
		if (!propertyInfo.CanWrite) return;
		this._mSetMethodInvoker = new MethodInvoker(propertyInfo.GetSetMethod(true));
	}

	public object GetValue(object o) {
		if (this._mGetter == null) {
			throw new NotSupportedException("Get method is not defined for this property.");
		}

		return this._mGetter(o);
	}

	public void SetValue(object o, object value) {
		if (this._mSetMethodInvoker == null) {
			throw new NotSupportedException("Set method is not defined for this property.");
		}

		this._mSetMethodInvoker.Invoke(o, new object[] { value });
	}

}
