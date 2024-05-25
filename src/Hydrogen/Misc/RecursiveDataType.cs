using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// A recursive data type is a data type for values that may contain other values of the same type. Data of recursive types is usually viewed as directed acyclic graphs.
/// A = 0
/// A(B) = 1 0
/// A(B(C)) = 1 1 0
/// A(B(C(D))) = 1 1 1 0
/// A(BC) = 2 0 0
/// A(BCD) = 3 0 0 0
/// A(BC(D)) = 2 0 1 0
/// </summary>
/// <typeparam name="TState"></typeparam>
public class RecursiveDataType<TState> : IEquatable<RecursiveDataType<TState>> {
	private readonly IExtendedList<RecursiveDataType<TState>> _subStates;

	public RecursiveDataType() : this(default, Enumerable.Empty<RecursiveDataType<TState>>()) {
	}

	public RecursiveDataType(TState state) : this(state, Enumerable.Empty<RecursiveDataType<TState>>()) {
	}

	public RecursiveDataType(TState state, IEnumerable<RecursiveDataType<TState>> subStates) {
		State = state;
		_subStates = subStates.ToExtendedList();
	}

	public TState State { get; set; }

	public IReadOnlyList<RecursiveDataType<TState>> SubStates => _subStates;

	public void AddSubStates(params TState[] subStates)  => AddSubStates(subStates.AsEnumerable());
	
	public void AddSubStates(IEnumerable<TState> childStates) 
		=> _subStates.AddRange(childStates.Select(x => new RecursiveDataType<TState>(x)));

	public void AddSubStates(params RecursiveDataType<TState>[] childStates) => AddSubStates(childStates.AsEnumerable());

	public void AddSubStates(IEnumerable<RecursiveDataType<TState>> childStates) 
		=> _subStates.AddRange(childStates);

	public static RecursiveDataType<TState> FromFlattened(IEnumerable<TState> flattenedStates, IEnumerable<int> flattenedSubStateCounts) {
		Guard.ArgumentNotNull(flattenedStates, nameof(flattenedStates));
		Guard.ArgumentNotNull(flattenedSubStateCounts, nameof(flattenedSubStateCounts));
		var flattenedStatesArr = (ReadOnlySpan<TState>)(flattenedStates as TState[] ?? flattenedStates.ToArray()).AsSpan();
		var flattenedSubStateCountsArr = (ReadOnlySpan<int>)(flattenedSubStateCounts as int[] ?? flattenedSubStateCounts.ToArray()).AsSpan();
		Guard.ArgumentGT(flattenedStatesArr.Length, 0, nameof(flattenedStates), "Flattened states and flattened sub-state counts must be equal");
		Guard.ArgumentEquals(flattenedStatesArr.Length, flattenedSubStateCountsArr.Length, nameof(flattenedStates), "Flattened states and flattened sub-state counts must be equal");
		return FromFlattenedInternal(ref flattenedStatesArr, ref flattenedSubStateCountsArr);			

		RecursiveDataType<TState> FromFlattenedInternal(ref ReadOnlySpan<TState> flattenedStateSpan, ref ReadOnlySpan<int> flattenedSubStateCountsSpan) {
			var state = flattenedStateSpan[0];
			var subStateCount = flattenedSubStateCountsSpan[0];
			flattenedStateSpan = flattenedStateSpan[1..];
			flattenedSubStateCountsSpan = flattenedSubStateCountsSpan[1..];
			var subStateL = new List<RecursiveDataType<TState>>();
			for(var i = 0; i < subStateCount; i++) {
				var subState = FromFlattenedInternal(ref flattenedStateSpan, ref flattenedSubStateCountsSpan);
				subStateL.Add(subState);
			}
			
			var rdt = new RecursiveDataType<TState> (state,subStateL);
			return rdt;
		}
	}

	public void Flatten(out TState[] flattenedStates, out int[] flattenedSubStateCounts) {
		var flattenedStatesL = new List<TState>();
		var flattedSubStateCountsL = new List<int>();
		var subStatesArr = SubStates.ToArray();
		flattenedStatesL.Add(State);
		flattedSubStateCountsL.Add(subStatesArr.Length);
		foreach(var subState in subStatesArr) {
			subState.Flatten(out var subStateFlattenedStates, out var subStateFlattenedSubStateCounts);
			flattenedStatesL.AddRange(subStateFlattenedStates);
			flattedSubStateCountsL.AddRange(subStateFlattenedSubStateCounts);
		}
		flattenedStates = flattenedStatesL.ToArray();
		flattenedSubStateCounts = flattedSubStateCountsL.ToArray();
	}
	
	public IEnumerable<TState> Flatten() =>
		State.ConcatWith(SubStates.SelectMany(x => x.Flatten()));

	public static RecursiveDataType<TState> Parse(TState root, Func<TState, long> countSubStates, Func<TState> getNextState) {
		return new RecursiveDataType<TState>(
			root, 
			Enumerable
				.Range(0, (int)countSubStates(root))
				.Select(_ => Parse(getNextState(), countSubStates, getNextState))
				.ToArray()
		);
	}

	public bool Equals(RecursiveDataType<TState> other) {
		if (ReferenceEquals(null, other))
			return false;
		if (ReferenceEquals(this, other))
			return true;
		return Equals(_subStates, other._subStates) && EqualityComparer<TState>.Default.Equals(State, other.State);
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj))
			return false;

		if (ReferenceEquals(this, obj))
			return true;
		
		if (obj.GetType() != this.GetType())
			return false;

		return Equals((RecursiveDataType<TState>)obj);
	}
	
	public override int GetHashCode() {
		return HashCode.Combine(_subStates.Select(x => x.GetHashCode()), State);
	}
}

