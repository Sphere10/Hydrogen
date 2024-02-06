using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Hydrogen;

/// <summary>
/// Base implementation of <see cref="IRecyclableList{T}"/>. 
/// </summary>
/// <typeparam name="T"></typeparam>
///<remarks>It is based on a singular-access list.</remarks>
public abstract class RecyclableListBase<T> : SingularListBase<T>, IRecyclableList<T> {

	public override long Count => ListCount - RecycledCount;

	public abstract long ListCount { get; }

	public abstract long RecycledCount { get; }

	public override long IndexOfL(T item) {
		var index = IndexOfInternal(item);
		if (IsRecycledInternal(index))
			return -1;
		return index;
	}

	public override bool Contains(T item) => IndexOfL(item) >= 0;

	public override T Read(long index) {
		Guard.Argument(!IsRecycledInternal(index), nameof(index), "Index is recycled");
		return ReadInternal(index);
	}

	public sealed override void Add(T item) => Add(item, out _);

	public virtual void Add(T item, out long index) {
		if (RecycledCount > 0) {
			index = ConsumeRecycledIndexInternal();
			UpdateInternal(index, item);
		} else {
			index = AddInternal(item);
		}
		UpdateVersion();
	}
	
	public override void Update(long index, T item) {
		Guard.Argument(!IsRecycledInternal(index), nameof(index), "Index is recycled");
		UpdateInternal(index, item);
		UpdateVersion();
	}

	public override void Insert(long index, T item)
		=> throw new NotSupportedException($"{GetType().Name} does not support Insert operation");

	public override bool Remove(T item) {
		var index = IndexOfInternal(item);
		if (index < 0)
			return false;
		RecycleInternal(index);
		UpdateVersion();
		return true;
	}

	public override void RemoveAt(long index) {
		Guard.Argument(!IsRecycledInternal(index), nameof(index), "Index is recycled");
		RecycleInternal(index);
		UpdateVersion();
	}

	public virtual void Recycle(long index) {
		Guard.Argument(!IsRecycledInternal(index), nameof(index), "Index is recycled");
		RecycleInternal(index);
		UpdateVersion();
	}

	public virtual bool IsRecycled(long index) {
		return IsRecycledInternal(index);
	}

	public override void Clear() {
		ClearInternal();
		UpdateVersion();
	}

	public override IEnumerator<T> GetEnumerator() {
		var version = base.Version;
		for (var i = 0L; i < ListCount; i++) {
			if (IsRecycledInternal(i))
				continue;
			CheckVersion(version);
			yield return ReadInternal(i);
		}
	}

	protected abstract long IndexOfInternal(T item);

	protected abstract long AddInternal(T item);

	protected abstract T ReadInternal(long index);

	protected abstract void UpdateInternal(long index, T item);

	protected abstract void RecycleInternal(long index);

	protected abstract bool IsRecycledInternal(long index);

	protected abstract long ConsumeRecycledIndexInternal();

	protected abstract void ClearInternal();

}
