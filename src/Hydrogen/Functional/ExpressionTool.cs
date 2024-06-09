// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq.Expressions;

// ReSharper disable CheckNamespace
namespace Tools;

/// <summary>
/// General purpose Expression utilities
/// </summary>
public static class Expressions {
	/// <summary>
	/// Create a function delegate representing a unary operation
	/// </summary>
	/// <typeparam name="TArg1">The parameter type</typeparam>
	/// <typeparam name="TResult">The return type</typeparam>
	/// <param name="body">Body factory</param>
	/// <returns>Compiled function delegate</returns>
	public static Func<TArg1, TResult> CreateExpression<TArg1, TResult>(
		Func<Expression, UnaryExpression> body) {
		var inp = Expression.Parameter(typeof(TArg1), "inp");
		try {
			return Expression.Lambda<Func<TArg1, TResult>>(body(inp), inp).Compile();
		} catch (Exception ex) {
			var msg = ex.Message; // avoid capture of ex itself
			return delegate { throw new InvalidOperationException(msg); };
		}
	}

	/// <summary>
	/// Create a function delegate representing a binary operation
	/// </summary>
	/// <typeparam name="TArg1">The first parameter type</typeparam>
	/// <typeparam name="TArg2">The second parameter type</typeparam>
	/// <typeparam name="TResult">The return type</typeparam>
	/// <param name="body">Body factory</param>
	/// <returns>Compiled function delegate</returns>
	public static Func<TArg1, TArg2, TResult> CreateExpression<TArg1, TArg2, TResult>(
		Func<Expression, Expression, BinaryExpression> body) {
		return CreateExpression<TArg1, TArg2, TResult>(body, false);
	}

	/// <summary>
	/// Create a function delegate representing a binary operation
	/// </summary>
	/// <param name="castArgsToResultOnFailure">
	/// If no matching operation is possible, attempt to convert
	/// TArg1 and TArg2 to TResult for a match? For example, there is no
	/// "decimal operator /(decimal, int)", but by converting TArg2 (int) to
	/// TResult (decimal) a match is found.
	/// </param>
	/// <typeparam name="TArg1">The first parameter type</typeparam>
	/// <typeparam name="TArg2">The second parameter type</typeparam>
	/// <typeparam name="TResult">The return type</typeparam>
	/// <param name="body">Body factory</param>
	/// <returns>Compiled function delegate</returns>
	public static Func<TArg1, TArg2, TResult> CreateExpression<TArg1, TArg2, TResult>(
		Func<Expression, Expression, BinaryExpression> body, bool castArgsToResultOnFailure) {
		var lhs = Expression.Parameter(typeof(TArg1), "lhs");
		var rhs = Expression.Parameter(typeof(TArg2), "rhs");
		try {
			try {
				return Expression.Lambda<Func<TArg1, TArg2, TResult>>(body(lhs, rhs), lhs, rhs).Compile();
			} catch (InvalidOperationException) {
				if (!castArgsToResultOnFailure ||
				    typeof(TArg1) == typeof(TResult) && // and the args aren't
				    typeof(TArg2) == typeof(TResult))
					throw; // already "TValue, TValue, TValue"...
				// convert both lhs and rhs to TResult (as appropriate)
				var castLhs = typeof(TArg1) == typeof(TResult) ? lhs : Expression.Convert(lhs, typeof(TResult)) as Expression;
				var castRhs = typeof(TArg2) == typeof(TResult) ? rhs : Expression.Convert(rhs, typeof(TResult)) as Expression;
				return Expression.Lambda<Func<TArg1, TArg2, TResult>>(body(castLhs, castRhs), lhs, rhs).Compile();
			}
		} catch (Exception ex) {
			var msg = ex.Message; // avoid capture of ex itself
			return delegate { throw new InvalidOperationException(msg); };
		}
	}
}
