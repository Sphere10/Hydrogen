using System.Threading.Tasks;

namespace Hydrogen.ObjectSpace.MetaData;

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
