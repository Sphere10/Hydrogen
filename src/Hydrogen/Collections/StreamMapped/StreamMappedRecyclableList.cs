// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
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

	public StreamMappedRecyclableList(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		long reservedStreams = 1,
		long freeIndexStoreStreamIndex = 0,
		long checksumIndexStreamIndex = 1,
		Endianness endianness = Endianness.LittleEndian, 
		bool autoLoad = false
	) : this(
		new StreamContainer(
			rootStream, 
			clusterSize,
			policy,
			reservedStreams,
			endianness, 
			false
		),
		itemSerializer, 
		itemComparer, 
		itemChecksummer,
		freeIndexStoreStreamIndex,
		checksumIndexStreamIndex,
		autoLoad
	)  {
		ObjectContainer.OwnsStreamContainer = true;
	}

	public StreamMappedRecyclableList(
		StreamContainer streamContainer,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		long freeIndexStoreStreamIndex = 0,
		long checksumIndexStreamIndex = 1,
		bool autoLoad = false
	) : this(
		BuildContainer(
			streamContainer, 
			itemSerializer, 
			itemChecksummer,
			freeIndexStoreStreamIndex,
			checksumIndexStreamIndex,
			out var freeIndexStore,
			out var checksumIndex
		), 
		freeIndexStore,
		checksumIndex,
		itemComparer,
		autoLoad
	) {
		OwnsContainer = true;
	}

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

	private static ObjectContainer<TItem> BuildContainer(
		StreamContainer streamContainer,
		IItemSerializer<TItem> itemSerializer,
		IItemChecksummer<TItem> itemChecksummer,
		long freeIndexStoreStreamIndex,
		long checksumIndexStreamIndex,
		out ObjectContainerFreeIndexStore freeIndexStore,
		out ObjectContainerIndex<TItem, int> checksumIndex
	) {
		var container = new ObjectContainer<TItem>(
			streamContainer, 
			itemSerializer, 
			streamContainer.Policy.HasFlag(StreamContainerPolicy.FastAllocate)
		);

		// Create free-index store
		freeIndexStore = new ObjectContainerFreeIndexStore(
			container,
			freeIndexStoreStreamIndex,
			0L
		);
		container.RegisterMetaDataProvider(freeIndexStore);

		// Create item checksum index (if applicable)
		if (itemChecksummer is not null) {
			checksumIndex = new ObjectContainerIndex<TItem, int>(
				container,
				checksumIndexStreamIndex,
				itemChecksummer.CalculateChecksum,
				EqualityComparer<int>.Default,
				PrimitiveSerializer<int>.Instance
			);
			container.RegisterMetaDataProvider( checksumIndex);
		} else {
			checksumIndex = null;
		}

		return container;
	}

}
