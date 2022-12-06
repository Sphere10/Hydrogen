using System.Threading.Tasks;

namespace Hydrogen;

public abstract class SyncRepositoryBase<TEntity, TIdentity> : RepositoryBase<TEntity, TIdentity>  {
	public override Task<bool> ContainsAsync(TIdentity identity) 
		=> Task.Run(() => Contains(identity));

	public override Task<(bool, TEntity)> TryGetAsync(TIdentity identity) 
		=> Task.Run( () => TryGet(identity, out var entity) ? (true, entity) : (false, entity) );

	public override Task CreateAsync(TEntity entity) 
		=> Task.Run(() => Create(entity));

	public override Task UpdateAsync(TEntity entity) 
		=> Task.Run(() => Update(entity));

	public override Task DeleteAsync(TIdentity identity) 
		=> Task.Run(() => Delete(identity));

	public override Task ClearAsync() 
		=> Task.Run(Clear);

	protected sealed override async ValueTask FreeManagedResourcesAsync() => FreeManagedResources();
}
