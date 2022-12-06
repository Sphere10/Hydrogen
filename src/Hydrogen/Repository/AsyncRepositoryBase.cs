namespace Hydrogen;

public abstract class AsyncRepositoryBase<TEntity, TIdentity> : RepositoryBase<TEntity, TIdentity>  {

	protected sealed override void FreeManagedResources() => FreeManagedResourcesAsync().AsTask().WaitSafe();

	public override bool Contains(TIdentity identity) 
		=> ContainsAsync(identity).ResultSafe();

	public override bool TryGet(TIdentity identity, out TEntity entity) {
		var result = TryGetAsync(identity).ResultSafe();
		entity = result.Item2;
		return result.Item1;
	}

	public override void Create(TEntity entity) 
		=> CreateAsync(entity).WaitSafe();

	public override void Update(TEntity entity) 
		=> UpdateAsync(entity).WaitSafe();

	public override void Delete(TIdentity identity) 
		=> DeleteAsync(identity).WaitSafe();

	public override void Clear()
		=> ClearAsync().WaitSafe();

}
