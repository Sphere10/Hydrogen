// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;


namespace Hydrogen;

/// <summary>
/// Represents a range of values. An IComparer{T} is used to compare specific
/// values with a start and end point. A range may be include or exclude each end
/// individually.
/// 
/// A range which is half-open but has the same start and end point is deemed to be empty,
/// e.g. [3,3) doesn't include 3. To create a range with a single value, use an inclusive
/// range, e.g. [3,3].
/// 
/// Ranges are always immutable - calls such as IncludeEnd() and ExcludeEnd() return a new
/// range without modifying this one.
/// </summary>
/// <remarks>
/// From Jon Skeet's library.
/// </remarks>
public sealed class ValueRange<T> {

	/// <summary>
	/// Constructs a new inclusive range using the default comparer
	/// </summary>
	public ValueRange(T start, T end)
		: this(start, end, Comparer<T>.Default, true, true) {
	}

	/// <summary>
	/// Constructs a new range including both ends using the specified comparer
	/// </summary>
	public ValueRange(T start, T end, IComparer<T> comparer)
		: this(start, end, comparer, true, true) {
	}

	/// <summary>
	/// Constructs a new range, including or excluding each end as specified,
	/// with the given comparer.
	/// </summary>
	public ValueRange(T start, T end, IComparer<T> comparer, bool includeStart, bool includeEnd, bool checkOrder = true) {
		if (checkOrder && comparer.Compare(start, end) > 0) {
			throw new ArgumentOutOfRangeException(nameof(end), "start must be lower than end according to comparer");
		}

		Start = start;
		End = end;
		Comparer = comparer;
		IncludesStart = includeStart;
		IncludesEnd = includeEnd;
	}

	/// <summary>
	/// The start of the range.
	/// </summary>
	public T Start { get; }

	/// <summary>
	/// The end of the range.
	/// </summary>
	public T End { get; }

	/// <summary>
	/// Comparer to use for comparisons
	/// </summary>
	public IComparer<T> Comparer { get; }

	/// <summary>
	/// Whether or not this range includes the start point
	/// </summary>
	public bool IncludesStart { get; }

	/// <summary>
	/// Whether or not this range includes the end point
	/// </summary>
	public bool IncludesEnd { get; }

	/// <summary>
	/// Returns a range with the same boundaries as this, but excluding the end point.
	/// When called on a range already excluding the end point, the original range is returned.
	/// </summary>
	public ValueRange<T> ExcludeEnd() {
		return !IncludesEnd ? this : new ValueRange<T>(Start, End, Comparer, IncludesStart, false);
	}

	/// <summary>
	/// Returns a range with the same boundaries as this, but excluding the start point.
	/// When called on a range already excluding the start point, the original range is returned.
	/// </summary>
	public ValueRange<T> ExcludeStart() {
		return !IncludesStart ? this : new ValueRange<T>(Start, End, Comparer, false, IncludesEnd);
	}

	/// <summary>
	/// Returns a range with the same boundaries as this, but including the end point.
	/// When called on a range already including the end point, the original range is returned.
	/// </summary>
	public ValueRange<T> IncludeEnd() {
		return IncludesEnd ? this : new ValueRange<T>(Start, End, Comparer, IncludesStart, true);
	}

	/// <summary>
	/// Returns a range with the same boundaries as this, but including the start point.
	/// When called on a range already including the start point, the original range is returned.
	/// </summary>
	public ValueRange<T> IncludeStart() {
		return IncludesStart ? this : new ValueRange<T>(Start, End, Comparer, true, IncludesEnd);
	}

	/// <summary>
	/// Returns whether or not the range contains the given value
	/// </summary>
	public bool Contains(T value) {
		var lowerBound = Comparer.Compare(value, Start);
		if (lowerBound < 0 || (lowerBound == 0 && !IncludesStart)) {
			return false;
		}
		var upperBound = Comparer.Compare(value, End);
		return upperBound < 0 || (upperBound == 0 && IncludesEnd);
	}

	/// <summary>
	/// Returns an iterator which begins at the start of this range,
	/// applying the given step delegate on each iteration until the 
	/// end is reached or passed. The start and end points are included
	/// or excluded according to this range.
	/// </summary>
	/// <param name="step">Delegate to apply to the "current value" on each iteration</param>
	public ValueRangeIterator<T> FromStart(Func<T, T> step) {
		return new ValueRangeIterator<T>(this, step);
	}

	/// <summary>
	/// Returns an iterator which begins at the end of this range,
	/// applying the given step delegate on each iteration until the 
	/// start is reached or passed. The start and end points are included
	/// or excluded according to this range.
	/// </summary>
	/// <param name="step">Delegate to apply to the "current value" on each iteration</param>
	public ValueRangeIterator<T> FromEnd(Func<T, T> step) {
		return new ValueRangeIterator<T>(this, step, false);
	}

	/// <summary>
	/// Returns an iterator which begins at the start of this range,
	/// adding the given step amount to the current value each iteration until the 
	/// end is reached or passed. The start and end points are included
	/// or excluded according to this range. This method does not check for
	/// the availability of an addition operator at compile-time; if you use it
	/// on a range where there is no such operator, it will fail at execution time.
	/// </summary>
	/// <param name="stepAmount">Amount to add on each iteration</param>
	public ValueRangeIterator<T> UpBy(T stepAmount) {
		return new ValueRangeIterator<T>(this, t => Tools.OperatorTool.Add(t, stepAmount));
	}

	/// <summary>
	/// Returns an iterator which begins at the end of this range,
	/// subtracting the given step amount to the current value each iteration until the 
	/// start is reached or passed. The start and end points are included
	/// or excluded according to this range. This method does not check for
	/// the availability of a subtraction operator at compile-time; if you use it
	/// on a range where there is no such operator, it will fail at execution time.
	/// </summary>
	/// <param name="stepAmount">Amount to subtract on each iteration. Note that
	/// this is subtracted, so in a range [0,10] you would pass +2 as this parameter
	/// to obtain the sequence (10, 8, 6, 4, 2, 0).
	/// </param>
	public ValueRangeIterator<T> DownBy(T stepAmount) {
		return new ValueRangeIterator<T>(this, t => Tools.OperatorTool.Subtract(t, stepAmount), false);
	}

	/// <summary>
	/// Returns an iterator which begins at the start of this range,
	/// adding the given step amount to the current value each iteration until the 
	/// end is reached or passed. The start and end points are included
	/// or excluded according to this range. This method does not check for
	/// the availability of an addition operator at compile-time; if you use it
	/// on a range where there is no such operator, it will fail at execution time.
	/// </summary>
	/// <param name="stepAmount">Amount to add on each iteration</param>
	public ValueRangeIterator<T> UpBy<TAmount>(TAmount stepAmount) {
		return new ValueRangeIterator<T>(this, t => Tools.OperatorTool.AddAlternative(t, stepAmount));
	}

	/// <summary>
	/// Returns an iterator which begins at the end of this range,
	/// subtracting the given step amount to the current value each iteration until the 
	/// start is reached or passed. The start and end points are included
	/// or excluded according to this range. This method does not check for
	/// the availability of a subtraction operator at compile-time; if you use it
	/// on a range where there is no such operator, it will fail at execution time.
	/// </summary>
	/// <param name="stepAmount">Amount to subtract on each iteration. Note that
	/// this is subtracted, so in a range [0,10] you would pass +2 as this parameter
	/// to obtain the sequence (10, 8, 6, 4, 2, 0).
	/// </param>
	public ValueRangeIterator<T> DownBy<TAmount>(TAmount stepAmount) {
		return new ValueRangeIterator<T>(this, t => Tools.OperatorTool.SubtractAlternative(t, stepAmount), false);
	}

	/// <summary>
	/// Returns an iterator which steps through the range, applying the specified
	/// step delegate on each iteration. The method determines whether to begin 
	/// at the start or end of the range based on whether the step delegate appears to go
	/// "up" or "down". The step delegate is applied to the start point. If the result is 
	/// more than the start point, the returned iterator begins at the start point; otherwise
	/// it begins at the end point.
	/// </summary>
	/// <param name="step">Delegate to apply to the "current value" on each iteration</param>
	public ValueRangeIterator<T> Step(Func<T, T> step) {
		if (step == null)
			throw new ArgumentNullException(nameof(step));
		var ascending = (Comparer.Compare(Start, step(Start)) < 0);

		return ascending ? FromStart(step) : FromEnd(step);
	}

	/// <summary>
	/// Returns an iterator which steps through the range, adding the specified amount
	/// on each iteration. If the step amount is logically negative, the returned iterator
	/// begins at the start point; otherwise it begins at the end point.
	/// This method does not check for
	/// the availability of an addition operator at compile-time; if you use it
	/// on a range where there is no such operator, it will fail at execution time.
	/// </summary>
	/// <param name="stepAmount">The amount to add on each iteration</param>
	public ValueRangeIterator<T> Step(T stepAmount) {
		return Step(t => Tools.OperatorTool.Add(t, stepAmount));
	}

	/// <summary>
	/// Returns an iterator which steps through the range, adding the specified amount
	/// on each iteration. If the step amount is logically negative, the returned iterator
	/// begins at the end point; otherwise it begins at the start point. This method
	/// is equivalent to Step(T stepAmount), but allows an alternative type to be used.
	/// The most common example of this is likely to be stepping a range of DateTimes
	/// by a TimeSpan.
	/// This method does not check for
	/// the availability of a suitable addition operator at compile-time; if you use it
	/// on a range where there is no such operator, it will fail at execution time.
	/// </summary>
	/// <param name="stepAmount">The amount to add on each iteration</param>
	public ValueRangeIterator<T> Step<TAmount>(TAmount stepAmount) {
		return Step(t => Tools.OperatorTool.AddAlternative(t, stepAmount));
	}

}
