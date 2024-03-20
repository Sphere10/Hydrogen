using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Adapts an <see cref="IExtendedList{T}"/> and a <see cref="IStack{T}"/> into a <see cref="IRecyclableList{T}"/>.
/// The <see cref="IStack{T}"/> is used to store the recycled indexes.
/// </summary>
/// <typeparam name="T"></typeparam>
///<remarks>It is based on a singular-access list.</remarks>
public class RecyclableListAdapter<T> : RecyclableListBase<T> {
	private readonly IExtendedList<T> _list;
	private readonly IStack<long> _recycledIndexStore;
	private readonly Func<long, bool> _isRecycledFunc;

	public RecyclableListAdapter(IEqualityComparer<T> comparer = null)
		:this(new ExtendedList<T>(comparer), new StackList<long>()) {
	}


	public RecyclableListAdapter(IExtendedList<T> list, IStack<long> recycledIndexStore) 
		: this(list, recycledIndexStore, recycledIndexStore.Contains) {
	}

	public RecyclableListAdapter(IExtendedList<T> list, IStack<long> recycledIndexStore, Func<long, bool> isRecycledFunc) {
		_list = list;
		_recycledIndexStore = recycledIndexStore;
		_isRecycledFunc = isRecycledFunc;
	}

	public override bool IsReadOnly => _list.IsReadOnly;

	public override long ListCount => _list.Count;

	public override long RecycledCount => _recycledIndexStore.Count;
	
	protected override long IndexOfInternal(T item) => _list.IndexOfL(item);

	protected override long AddInternal(T item)  {
		var index = _list.Count;
		_list.Add(item);
		return index;
	}

	protected override T ReadInternal(long index) => _list.Read(index);

	protected override void UpdateInternal(long index, T item) => _list.Update(index, item);

	protected override void RecycleInternal(long index) {
		_list.Update(index, default);
		_recycledIndexStore.Push(index);
	}

	protected override bool IsRecycledInternal(long index) => _isRecycledFunc(index);

	protected override long ConsumeRecycledIndexInternal() => _recycledIndexStore.Pop();

	protected override void ClearInternal() {
		_recycledIndexStore.Clear();
		_list.Clear();
	}


}
