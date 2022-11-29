//-----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Hydrogen {

	public static class ExpressionExtensions {

		public static MemberInfo ResolveMember<T, V>(this Expression<Func<T, V>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
				throw new InvalidOperationException("Expression must be a member expression");

			return memberExpression.Member;
		}

	}
}
