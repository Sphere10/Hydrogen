// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

internal abstract class MetaDataStoreDecorator<TData, TInner> : IMetaDataStore<TData> where TInner : IMetaDataStore<TData> {
	public event EventHandlerEx<object> Loading { add => InnerStore.Loading += value; remove => InnerStore.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InnerStore.Loaded += value; remove => InnerStore.Loaded -= value; }
	
	internal TInner InnerStore;

	protected MetaDataStoreDecorator(TInner innerStore) {
		InnerStore = innerStore;
	}

	public virtual ObjectContainer Container => InnerStore.Container;

	public virtual long Count => InnerStore.Count;

	public virtual bool RequiresLoad => InnerStore.RequiresLoad;

	public virtual void Load() => InnerStore.Load();

	public virtual Task LoadAsync() => InnerStore.LoadAsync();

	public virtual TData Read(long index) => InnerStore.Read(index);

	public virtual byte[] ReadBytes(long index) => InnerStore.ReadBytes(index);

	public virtual void Add(long index, TData data) => InnerStore.Add(index, data);

	public virtual void Update(long index, TData data) => InnerStore.Update(index, data);

	public virtual void Insert(long index, TData data) => InnerStore.Insert(index, data);

	public virtual void Remove(long index) => InnerStore.Remove(index);

	public virtual void Reap(long index) => InnerStore.Reap(index);

	public virtual void Clear() => InnerStore.Clear();

	public virtual void Dispose() => InnerStore.Dispose();
}

internal abstract class MetaDataStoreDecorator<TData> : MetaDataStoreDecorator<TData, IMetaDataStore<TData>>  {

	protected MetaDataStoreDecorator(IMetaDataStore<TData> innerStore) : base(innerStore) {
	}
}
