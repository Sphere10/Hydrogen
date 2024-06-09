// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq.Expressions;

namespace Hydrogen;

/// <summary>
/// Provides standard operators (such as addition) that operate over operands of
/// different types. For operators, the return type is assumed to match the first
/// operand.
/// </summary>
/// <seealso cref="Operator&lt;T&gt;"/>
/// <seealso cref="OperatorTool"/>
public static class Operator<TValue, TResult> {
	/// <summary>
	/// Returns a delegate to convert a value between two types; this delegate will throw
	/// an InvalidOperationException if the type T does not provide a suitable cast, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this cast.
	/// </summary>
	public static Func<TValue, TResult> Convert { get; }

	static Operator() {
		Convert = Tools.Expressions.CreateExpression<TValue, TResult>(body => Expression.Convert(body, typeof(TResult)));
		Add = Tools.Expressions.CreateExpression<TResult, TValue, TResult>(Expression.Add, true);
		Subtract = Tools.Expressions.CreateExpression<TResult, TValue, TResult>(Expression.Subtract, true);
		Multiply = Tools.Expressions.CreateExpression<TResult, TValue, TResult>(Expression.Multiply, true);
		Divide = Tools.Expressions.CreateExpression<TResult, TValue, TResult>(Expression.Divide, true);
	}

	/// <summary>
	/// Returns a delegate to evaluate binary addition (+) for the given types; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<TResult, TValue, TResult> Add { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary subtraction (-) for the given types; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<TResult, TValue, TResult> Subtract { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary multiplication (*) for the given types; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<TResult, TValue, TResult> Multiply { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary division (/) for the given types; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<TResult, TValue, TResult> Divide { get; }
}


/// <summary>
/// Provides standard operators (such as addition) over a single type
/// </summary>
public static class Operator<T> {

	internal static INullOp<T> NullOp { get; }

	/// <summary>
	/// Returns the zero value for value-types (even full Nullable&lt;TInner&gt;) - or null for reference types
	/// </summary>
	public static T Zero { get; }

	/// <summary>
	/// Returns a delegate to evaluate unary negation (-) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T> Negate { get; }

	/// <summary>
	/// Returns a delegate to evaluate bitwise not (~) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T> Not { get; }

	/// <summary>
	/// Returns a delegate to evaluate bitwise or (|) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, T> Or { get; }

	/// <summary>
	/// Returns a delegate to evaluate bitwise and (&amp;) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, T> And { get; }

	/// <summary>
	/// Returns a delegate to evaluate bitwise xor (^) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, T> Xor { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary addition (+) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, T> Add { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary subtraction (-) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, T> Subtract { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary multiplication (*) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, T> Multiply { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary division (/) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, T> Divide { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary equality (==) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, bool> Equal { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary inequality (!=) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, bool> NotEqual { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary greater-then (&gt;) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, bool> GreaterThan { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary less-than (&lt;) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, bool> LessThan { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary greater-than-or-equal (&gt;=) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, bool> GreaterThanOrEqual { get; }

	/// <summary>
	/// Returns a delegate to evaluate binary less-than-or-equal (&lt;=) for the given type; this delegate will throw
	/// an InvalidOperationException if the type T does not provide this operator, or for
	/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
	/// </summary>
	public static Func<T, T, bool> LessThanOrEqual { get; }

	static Operator() {
		Add = Tools.Expressions.CreateExpression<T, T, T>(Expression.Add);
		Subtract = Tools.Expressions.CreateExpression<T, T, T>(Expression.Subtract);
		Divide = Tools.Expressions.CreateExpression<T, T, T>(Expression.Divide);
		Multiply = Tools.Expressions.CreateExpression<T, T, T>(Expression.Multiply);

		GreaterThan = Tools.Expressions.CreateExpression<T, T, bool>(Expression.GreaterThan);
		GreaterThanOrEqual = Tools.Expressions.CreateExpression<T, T, bool>(Expression.GreaterThanOrEqual);
		LessThan = Tools.Expressions.CreateExpression<T, T, bool>(Expression.LessThan);
		LessThanOrEqual = Tools.Expressions.CreateExpression<T, T, bool>(Expression.LessThanOrEqual);
		Equal = Tools.Expressions.CreateExpression<T, T, bool>(Expression.Equal);
		NotEqual = Tools.Expressions.CreateExpression<T, T, bool>(Expression.NotEqual);

		Negate = Tools.Expressions.CreateExpression<T, T>(Expression.Negate);
		And = Tools.Expressions.CreateExpression<T, T, T>(Expression.And);
		Or = Tools.Expressions.CreateExpression<T, T, T>(Expression.Or);
		Not = Tools.Expressions.CreateExpression<T, T>(Expression.Not);
		Xor = Tools.Expressions.CreateExpression<T, T, T>(Expression.ExclusiveOr);

		var typeT = typeof(T);
		if (typeT.IsValueType && typeT.IsGenericType && (typeT.GetGenericTypeDefinition() == typeof(Nullable<>))) {
			// get the *inner* zero (not a null Nullable<TValue>, but default(TValue))
			var nullType = typeT.GetGenericArguments()[0];
			Zero = (T)Activator.CreateInstance(nullType);
			NullOp = (INullOp<T>)Activator.CreateInstance(
				typeof(StructNullOp<>).MakeGenericType(nullType));
		} else {
			Zero = default(T);
			if (typeT.IsValueType) {
				NullOp = (INullOp<T>)Activator.CreateInstance(
					typeof(StructNullOp<>).MakeGenericType(typeT));
			} else {
				NullOp = (INullOp<T>)Activator.CreateInstance(
					typeof(ClassNullOp<>).MakeGenericType(typeT));
			}
		}
	}

}
