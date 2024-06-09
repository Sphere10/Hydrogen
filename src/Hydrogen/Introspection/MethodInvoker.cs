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
using System.Linq.Expressions;

namespace Hydrogen.FastReflection;

public class MethodInvoker {
	private readonly Func<object, object[], object> _mInvoker;

	public MethodInfo MethodInfo { get; private set; }

	public MethodInvoker(MethodInfo methodInfo) {
		this.MethodInfo = methodInfo;
		this._mInvoker = CreateInvokeDelegate(methodInfo);
	}

	public object Invoke(object instance, params object[] parameters) {
		return this._mInvoker(instance, parameters);
	}

	private static Func<object, object[], object> CreateInvokeDelegate(MethodInfo methodInfo) {
		// Target: ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)

		// parameters to execute
		var instanceParameter = Expression.Parameter(typeof(object), "instance");
		var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

		// build parameter list
		var parameterExpressions = new List<Expression>();
		var paramInfos = methodInfo.GetParameters();
		for (int i = 0; i < paramInfos.Length; i++) {
			// (Ti)parameters[i]
			BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
			UnaryExpression valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);

			parameterExpressions.Add(valueCast);
		}

		// non-instance for static method, or ((TInstance)instance)
		var instanceCast = methodInfo.IsStatic ? null : Expression.Convert(instanceParameter, methodInfo.ReflectedType);

		// static invoke or ((TInstance)instance).Method
		var methodCall = Expression.Call(instanceCast, methodInfo, parameterExpressions);

		// ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)
		if (methodCall.Type == typeof(void)) {
			var lambda = Expression.Lambda<Action<object, object[]>>(methodCall, instanceParameter, parametersParameter);

			Action<object, object[]> execute = lambda.Compile();
			return (instance, parameters) => {
				execute(instance, parameters);
				return null;
			};
		} else {
			var castMethodCall = Expression.Convert(methodCall, typeof(object));
			var lambda = Expression.Lambda<Func<object, object[], object>>(
				castMethodCall,
				instanceParameter,
				parametersParameter);

			return lambda.Compile();
		}
	}

}
