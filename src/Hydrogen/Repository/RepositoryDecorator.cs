// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

public class RepositoryDecorator<TEntity, TIdentity, TConcrete> : IRepository<TEntity, TIdentity> where TConcrete : IRepository<TEntity, TIdentity> {
	protected TConcrete InternalCollection;

	public event EventHandlerEx<TEntity> Changing {
		add => InternalCollection.Changing += value;
		remove => InternalCollection.Changing -= value;
	}

	public event EventHandlerEx<TEntity> Changed {
		add => InternalCollection.Changed += value;
		remove => InternalCollection.Changed -= value;
	}

	public event EventHandlerEx<TEntity> Saving {
		add => InternalCollection.Saving += value;
		remove => InternalCollection.Saving -= value;
	}

	public event EventHandlerEx<TEntity> Saved {
		add => InternalCollection.Saved += value;
		remove => InternalCollection.Saved -= value;
	}

	public event EventHandlerEx Clearing {
		add => InternalCollection.Clearing += value;
		remove => InternalCollection.Clearing -= value;
	}

	public event EventHandlerEx Cleared {
		add => InternalCollection.Cleared += value;
		remove => InternalCollection.Cleared -= value;
	}

	public event EventHandlerEx<TEntity> Adding {
		add => InternalCollection.Adding += value;
		remove => InternalCollection.Adding -= value;
	}

	public event EventHandlerEx<TEntity> Added {
		add => InternalCollection.Added += value;
		remove => InternalCollection.Added -= value;
	}

	public event EventHandlerEx<TEntity> Updating {
		add => InternalCollection.Updating += value;
		remove => InternalCollection.Updating -= value;
	}

	public event EventHandlerEx<TEntity> Updated {
		add => InternalCollection.Updated += value;
		remove => InternalCollection.Updated -= value;
	}

	public RepositoryDecorator(TConcrete innerRepository) {
		Guard.ArgumentNotNull(innerRepository, nameof(innerRepository));
		InternalCollection = innerRepository;
	}

	public virtual bool Contains(TIdentity identity) => InternalCollection.Contains(identity);

	public virtual Task<bool> ContainsAsync(TIdentity identity) => InternalCollection.ContainsAsync(identity);

	public virtual bool TryGet(TIdentity identity, out TEntity entity) => InternalCollection.TryGet(identity, out entity);

	public virtual Task<(bool, TEntity)> TryGetAsync(TIdentity identity) => InternalCollection.TryGetAsync(identity);

	public virtual void Create(TEntity entity) => InternalCollection.Create(entity);

	public virtual Task CreateAsync(TEntity entity) => InternalCollection.CreateAsync(entity);

	public virtual void Update(TEntity entity) => InternalCollection.Update(entity);

	public virtual Task UpdateAsync(TEntity entity) => InternalCollection.UpdateAsync(entity);

	public virtual void Delete(TIdentity identity) => InternalCollection.Delete(identity);

	public virtual Task DeleteAsync(TIdentity identity) => InternalCollection.DeleteAsync(identity);

	public virtual void Clear() => InternalCollection.Clear();

	public virtual Task ClearAsync() => InternalCollection.ClearAsync();

	public virtual void Dispose() => InternalCollection.Dispose();

	public virtual ValueTask DisposeAsync() => InternalCollection.DisposeAsync();

}


public class RepositoryDecorator<TEntity, TIdentity> : RepositoryDecorator<TEntity, TIdentity, IRepository<TEntity, TIdentity>> {

	public RepositoryDecorator(IRepository<TEntity, TIdentity> innerRepository) : base(innerRepository) {
	}
}
