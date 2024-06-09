// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

public abstract class RepositoryBase<TEntity, TIdentity> : Disposable, IRepository<TEntity, TIdentity> {
	public event EventHandlerEx<TEntity> Changing;
	public event EventHandlerEx<TEntity> Changed;
	public event EventHandlerEx<TEntity> Saving;
	public event EventHandlerEx<TEntity> Saved;
	public event EventHandlerEx Clearing;
	public event EventHandlerEx Cleared;
	public event EventHandlerEx<TEntity> Adding;
	public event EventHandlerEx<TEntity> Added;
	public event EventHandlerEx<TEntity> Updating;
	public event EventHandlerEx<TEntity> Updated;

	public virtual bool Contains(TIdentity identity)
		=> TryGet(identity, out _);

	public virtual async Task<bool> ContainsAsync(TIdentity identity)
		=> (await TryGetAsync(identity)).Item1;

	public abstract bool TryGet(TIdentity identity, out TEntity entity);

	public abstract Task<(bool, TEntity)> TryGetAsync(TIdentity identity);

	public abstract void Create(TEntity entity);

	public abstract Task CreateAsync(TEntity entity);

	public abstract void Update(TEntity entity);

	public abstract Task UpdateAsync(TEntity entity);

	public abstract void Delete(TIdentity identity);

	public abstract Task DeleteAsync(TIdentity identity);

	public abstract void Clear();

	public abstract Task ClearAsync();

	protected virtual void OnChanging(TEntity entity) {
	}

	protected virtual void OnChanged(TEntity entity) {
	}

	protected virtual void OnSaving(TEntity entity) {
	}

	protected virtual void OnSaved(TEntity entity) {
	}

	protected virtual void OnClearing() {
	}

	protected virtual void OnCleared() {
	}

	protected virtual void OnAdding(TEntity entity) {
	}

	protected virtual void OnAdded(TEntity entity) {
	}

	protected virtual void OnUpdating(TEntity entity) {
	}

	protected virtual void OnUpdated(TEntity entity) {
	}

	protected void NotifyChanging(TEntity entity) {
		OnChanging(entity);
		Changing?.Invoke(entity);
	}

	protected void NotifyChanged(TEntity entity) {
		OnChanged(entity);
		Changed?.Invoke(entity);
	}

	protected void NotifySaving(TEntity entity) {
		OnSaving(entity);
		Saving?.Invoke(entity);
	}

	protected void NotifySaved(TEntity entity) {
		OnSaved(entity);
		Saved?.Invoke(entity);
	}

	protected void NotifyClearing() {
		OnClearing();
		Clearing?.Invoke();
	}

	protected void NotifyCleared() {
		OnCleared();
		Cleared?.Invoke();
	}

	protected void NotifyAdding(TEntity entity) {
		OnAdding(entity);
		Adding?.Invoke(entity);
	}

	protected void NotifyAdded(TEntity entity) {
		OnAdded(entity);
		Added?.Invoke(entity);
	}

	protected void NotifyUpdating(TEntity entity) {
		OnUpdating(entity);
		Updating?.Invoke(entity);
	}

	protected void NotifyUpdated(TEntity entity) {
		OnUpdated(entity);
		Updated?.Invoke(entity);
	}

}
