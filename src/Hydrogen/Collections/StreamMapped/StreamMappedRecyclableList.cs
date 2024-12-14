// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Threading.Tasks;
using Hydrogen;


public class StreamMappedRecyclableList<TItem> :  RecyclableListBase<TItem>, IStreamMappedRecyclableList<TItem>, IStreamMappedRecylableList {
	public event EventHandlerEx<object> Loading { add => ObjectStream.Loading += value; remove => ObjectStream.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => ObjectStream.Loaded += value; remove => ObjectStream.Loaded -= value; }

	private readonly RecyclableIndexIndex _recylableIndexIndex;
	private readonly ProjectionIndex<TItem, int> _projectionChecksumIndex;

	public StreamMappedRecyclableList(
		ObjectStream<TItem> objectStream,
		string recylcableIndexIndexName, 
		string optionalItemChecksumIndexName = null,
		IEqualityComparer<TItem> itemComparer = null, 
		bool autoLoad = false 
	) {
		Guard.ArgumentNotNull(objectStream, nameof(objectStream));
		ObjectStream = objectStream;
		_recylableIndexIndex = (RecyclableIndexIndex)objectStream.Streams.Attachments[recylcableIndexIndexName];

		Guard.Against(!string.IsNullOrEmpty(optionalItemChecksumIndexName) && !objectStream.Streams.Attachments.ContainsKey(optionalItemChecksumIndexName), $"No index '{optionalItemChecksumIndexName}' was found in object stream"); 
		if (!string.IsNullOrEmpty(optionalItemChecksumIndexName)) 
			_projectionChecksumIndex = (ProjectionIndex<TItem, int>)objectStream.Streams.Attachments[optionalItemChecksumIndexName]; // use for O(1) exists

		ItemComparer = itemComparer ?? EqualityComparer<TItem>.Default;

		if (autoLoad && RequiresLoad)
			Load();
	}

	public bool OwnsContainer { get; set; }

	public override long ListCount => ObjectStream.Count;

	public override long RecycledCount => _recylableIndexIndex.Store.Count;
	
	public bool RequiresLoad => ObjectStream.RequiresLoad;
	
	public ObjectStream<TItem> ObjectStream { get; }

	ObjectStream IStreamMappedCollection.ObjectStream => ObjectStream;

	public IItemSerializer<TItem> ItemSerializer => ObjectStream.ItemSerializer;

	public IEqualityComparer<TItem> ItemComparer { get; }

	public void Load() => ObjectStream.Load();

	public Task LoadAsync() => ObjectStream.LoadAsync();

	public override long IndexOfL(TItem item) {
		using (ObjectStream.EnterAccessScope())
			return base.IndexOfL(item);
	}

	public override TItem Read(long index) {
		using (ObjectStream.EnterAccessScope())
			return base.Read(index);
	}

	public override void Add(TItem item, out long index) {
		using (ObjectStream.EnterAccessScope())
			base.Add(item, out index);
	}

	public override void Update(long index, TItem item) {
		using (ObjectStream.EnterAccessScope())
			base.Update(index, item);
	}

	public override bool Remove(TItem item) {
		using (ObjectStream.EnterAccessScope())
			return base.Remove(item);
	}

	public override void RemoveAt(long index) {
		using (ObjectStream.EnterAccessScope())
			base.RemoveAt(index);
	}

	public override void Recycle(long index) {
		using (ObjectStream.EnterAccessScope())
			base.Recycle(index);
	}

	public override bool IsRecycled(long index) {
		using (ObjectStream.EnterAccessScope())
			return base.IsRecycled(index);
	}

	public override void Clear() {
		using (ObjectStream.EnterAccessScope())
			base.Clear();
	}

	public override IEnumerator<TItem> GetEnumerator() {
		var scope = ObjectStream.EnterAccessScope();
		return base.GetEnumerator().OnDispose(scope.Dispose);
	}

	public void Dispose() {
		if (OwnsContainer)
			ObjectStream.Dispose();
	}

	protected override long IndexOfInternal(TItem item)  {
		using (ObjectStream.EnterAccessScope()) {
			var indicesToCheck =
				_projectionChecksumIndex != null ?
					_projectionChecksumIndex.Values[_projectionChecksumIndex.ApplyProjection(item)] :
					Tools.Collection.RangeL(0L, ListCount);

			foreach (var index in indicesToCheck) {
				if (IsRecycledInternal(index))
					continue;
				if (ItemComparer.Equals(item, ReadInternal(index)))
					return index;
			}
			return -1L;
		}
	}

	protected override TItem ReadInternal(long index)
		=> ObjectStream.LoadItem(index);

	protected override long AddInternal(TItem item) {
		var index = ObjectStream.Count;
		ObjectStream.SaveItem(index, item, ObjectStreamOperationType.Add);
		return index;
	}

	protected override void UpdateInternal(long index, TItem item) 
		=> ObjectStream.SaveItem(index, item, ObjectStreamOperationType.Update);

	protected override bool IsRecycledInternal(long index) 
		=> ObjectStream.GetItemDescriptor(index).Traits.HasFlag(ClusteredStreamTraits.Reaped);

	protected override void RecycleInternal(long index) {
		ObjectStream.ReapItem(index); // note: _recylableIndexIndex listens to recycle's in ObjectStream
	}

	protected override long ConsumeRecycledIndexInternal() 
		=> _recylableIndexIndex.Store.Pop();

	protected override void ClearInternal() {
		using (ObjectStream.EnterAccessScope()) {
			ObjectStream.Clear();
			_recylableIndexIndex.Store.Clear();
		}
	}

}
