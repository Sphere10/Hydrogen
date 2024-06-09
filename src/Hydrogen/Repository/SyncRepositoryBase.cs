// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

public abstract class SyncRepositoryBase<TEntity, TIdentity> : RepositoryBase<TEntity, TIdentity> {
	public override Task<bool> ContainsAsync(TIdentity identity)
		=> Task.Run(() => Contains(identity));

	public override Task<(bool, TEntity)> TryGetAsync(TIdentity identity)
		=> Task.Run(() => TryGet(identity, out var entity) ? (true, entity) : (false, entity)).ContinueOnSameThread();

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
