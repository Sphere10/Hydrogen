// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public abstract class AsyncRepositoryBase<TEntity, TIdentity> : RepositoryBase<TEntity, TIdentity> {

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
