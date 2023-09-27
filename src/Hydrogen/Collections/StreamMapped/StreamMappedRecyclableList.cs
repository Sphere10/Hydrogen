﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hydrogen;

public class StreamMappedRecyclableList<TItem> :  RecyclableListBase<TItem>, IStreamMappedRecyclableList<TItem> {
	public event EventHandlerEx<object> Loading { add => ObjectContainer.Loading += value; remove => ObjectContainer.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => ObjectContainer.Loaded += value; remove => ObjectContainer.Loaded -= value; }

	private readonly ObjectContainerFreeIndexStore _freeIndexStore;
	private readonly ObjectContainerIndex<TItem, int> _checksumIndex;
	private IDisposable _accessScope;

	internal StreamMappedRecyclableList(
		ObjectContainer<TItem> container, 
		ObjectContainerFreeIndexStore freeIndexStore, 
		ObjectContainerIndex<TItem, int> checksumIndex,
		IEqualityComparer<TItem> itemComparer = null,
		bool autoLoad = false
	) {
		Guard.ArgumentNotNull(container, nameof(container));
		Guard.ArgumentNotNull(freeIndexStore, nameof(freeIndexStore));
		ObjectContainer = container;
		_freeIndexStore = freeIndexStore;
		_checksumIndex = checksumIndex;
		ItemComparer = itemComparer ?? EqualityComparer<TItem>.Default;

		if (autoLoad && RequiresLoad)
			Load();
	}

	public bool OwnsContainer { get; set; }

	public override long ListCount => ObjectContainer.Count;

	public override long RecycledCount => _freeIndexStore.Stack.Count;
	
	public bool RequiresLoad => ObjectContainer.RequiresLoad;
	
	public ObjectContainer<TItem> ObjectContainer { get; }

	public IItemSerializer<TItem> ItemSerializer => ObjectContainer.ItemSerializer;

	public IEqualityComparer<TItem> ItemComparer { get; }

	public void Load() => ObjectContainer.Load();

	public Task LoadAsync() => ObjectContainer.LoadAsync();

	public override long IndexOfL(TItem item) {
		using (ObjectContainer.EnterAccessScope())
			return base.IndexOfL(item);
	}

	public override TItem Read(long index) {
		using (ObjectContainer.EnterAccessScope())
			return base.Read(index);
	}

	public override void Add(TItem item) {
		using (ObjectContainer.EnterAccessScope())
			base.Add(item);
	}

	public override void Update(long index, TItem item) {
		using (ObjectContainer.EnterAccessScope())
			base.Update(index, item);
	}

	public override bool Remove(TItem item) {
		using (ObjectContainer.EnterAccessScope())
			return base.Remove(item);
	}

	public override void RemoveAt(long index) {
		using (ObjectContainer.EnterAccessScope())
			base.RemoveAt(index);
	}

	public override void Recycle(long index) {
		using (ObjectContainer.EnterAccessScope())
			base.Recycle(index);
	}

	public override bool IsRecycled(long index) {
		using (ObjectContainer.EnterAccessScope())
			return base.IsRecycled(index);
	}

	public override void Clear() {
		using (ObjectContainer.EnterAccessScope())
			base.Clear();
	}

	public override IEnumerator<TItem> GetEnumerator() {
		var scope = ObjectContainer.EnterAccessScope();
		return base.GetEnumerator().OnDispose(scope.Dispose);
	}

	public void Dispose() {
		if (OwnsContainer)
			ObjectContainer.Dispose();
	}

	protected override long IndexOfInternal(TItem item)  {
		var indicesToCheck =
			_checksumIndex != null ?
				_checksumIndex.Lookup[_checksumIndex.CalculateKey(item)] :
				Tools.Collection.RangeL(0L, ListCount);

		foreach (var index in indicesToCheck) {
			if (IsRecycledInternal(index))
				continue;
			if (ItemComparer.Equals(item, ReadInternal(index)))
				return index;
		}
		return -1L;
	}

	protected override TItem ReadInternal(long index)
		=> ObjectContainer.LoadItem(index);

	protected override void AddInternal(TItem item) 
		=> ObjectContainer.SaveItem(ObjectContainer.Count, item, ObjectContainerOperationType.Add);

	protected override void UpdateInternal(long index, TItem item) 
		=> ObjectContainer.SaveItem(index, item, ObjectContainerOperationType.Update);

	protected override bool IsRecycledInternal(long index) 
		=> ObjectContainer.GetItemDescriptor(index).Traits.HasFlag(ClusteredStreamTraits.Reaped);

	protected override void RecycleInternal(long index) {
		ObjectContainer.ReapItem(index); // note: _freeIndexStore listens to recycle's in ObjectContainer
	}

	protected override long ConsumeRecycledIndexInternal() 
		=> _freeIndexStore.Stack.Pop();

	protected override void ClearInternal() {
		ObjectContainer.Clear();
		_freeIndexStore.Stack.Clear();
	}
	
}
