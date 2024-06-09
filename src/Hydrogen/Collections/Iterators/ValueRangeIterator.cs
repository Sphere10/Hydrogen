// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Iterates over a range. Despite its name, this implements IEnumerable{T} rather than
/// IEnumerator{T} - it just sounds better, frankly.
/// </summary>
/// <remarks>
/// Acknowledgement: Mostly written by Jon Skeet.
/// </remarks>
public readonly struct ValueRangeIterator<T> : IEnumerable<T> {

	/// <summary>
	/// Creates an ascending iterator over the given range with the given step function
	/// </summary>
	public ValueRangeIterator(ValueRange<T> range, Func<T, T> step)
		: this(range, step, true) {
	}

	/// <summary>
	/// Creates an iterator over the given range with the given step function,
	/// with the specified direction.
	/// </summary>
	public ValueRangeIterator(ValueRange<T> range, Func<T, T> step, bool ascending) {
		if (step == null) throw new ArgumentNullException(nameof(step));

		if ((ascending && range.Comparer.Compare(range.Start, step(range.Start)) >= 0) ||
		    (!ascending && range.Comparer.Compare(range.End, step(range.End)) <= 0)) {
			throw new ArgumentException("step does nothing, or progresses the wrong way");
		}
		this.Ascending = ascending;
		this.Range = range;
		this.Step = step;
	}

	/// <summary>
	/// Returns the range this object iterates over
	/// </summary>
	public ValueRange<T> Range { get; }


	/// <summary>
	/// Returns the step function used for this range
	/// </summary>
	public Func<T, T> Step { get; }


	/// <summary>
	/// Returns whether or not this iterator works up from the start point (ascending)
	/// or down from the end point (descending)
	/// </summary>
	public bool Ascending { get; }

	/// <summary>
	/// Returns an IEnumerator{T} running over the range.
	/// </summary>
	public IEnumerator<T> GetEnumerator() {
		// A descending range effectively has the start and end points (and inclusions)
		// reversed, and a reverse comparer.
		var includesStart = Ascending ? Range.IncludesStart : Range.IncludesEnd;
		var includesEnd = Ascending ? Range.IncludesEnd : Range.IncludesStart;
		var start = Ascending ? Range.Start : Range.End;
		var end = Ascending ? Range.End : Range.Start;
		var comparer = Ascending ? Range.Comparer : Range.Comparer.AsInverted();

		// Now we can use our local version of the range variables to iterate
		var value = start;

		if (includesStart) {
			// Deal with possibility that start point = end point
			if (includesEnd || comparer.Compare(value, end) < 0) {
				yield return value;
			}
		}
		value = Step(value);

		while (comparer.Compare(value, end) < 0) {
			yield return value;
			value = Step(value);
		}

		// We've already performed a step, therefore we can't
		// still be at the start point
		if (includesEnd && comparer.Compare(value, end) == 0) {
			yield return value;
		}
	}

	/// <summary>
	/// Returns an IEnumerator running over the range.
	/// </summary>
	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
