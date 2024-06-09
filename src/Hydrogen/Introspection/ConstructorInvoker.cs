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

public class ConstructorInvoker {
	private readonly Func<object[], object> _mInvoker;

	public ConstructorInfo ConstructorInfo { get; private set; }

	public ConstructorInvoker(ConstructorInfo constructorInfo) {
		this.ConstructorInfo = constructorInfo;
		this._mInvoker = InitializeInvoker(constructorInfo);
	}

	private Func<object[], object> InitializeInvoker(ConstructorInfo constructorInfo) {
		// Target: (object)new T((T0)parameters[0], (T1)parameters[1], ...)

		// parameters to execute
		var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

		// build parameter list
		var parameterExpressions = new List<Expression>();
		var paramInfos = constructorInfo.GetParameters();
		for (var i = 0; i < paramInfos.Length; i++) {
			// (Ti)parameters[i]
			var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
			var valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);

			parameterExpressions.Add(valueCast);
		}

		// new T((T0)parameters[0], (T1)parameters[1], ...)
		var instanceCreate = Expression.New(constructorInfo, parameterExpressions);

		// (object)new T((T0)parameters[0], (T1)parameters[1], ...)
		var instanceCreateCast = Expression.Convert(instanceCreate, typeof(object));

		var lambda = Expression.Lambda<Func<object[], object>>(instanceCreateCast, parametersParameter);

		return lambda.Compile();
	}

	public object Invoke(params object[] parameters) {
		return this._mInvoker(parameters);
	}

}
