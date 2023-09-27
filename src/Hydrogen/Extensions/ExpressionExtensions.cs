// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
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
