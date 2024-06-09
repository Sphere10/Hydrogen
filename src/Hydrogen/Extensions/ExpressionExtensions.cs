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

namespace Hydrogen;

public static class ExpressionExtensions {

	public static MemberInfo ResolveMember<T, V>(this Expression<Func<T, V>> expression) {
		var memberExpression = expression.Body as MemberExpression;
		if (memberExpression == null)
			throw new InvalidOperationException("Expression must be a member expression");

		return memberExpression.Member;
	}

}
