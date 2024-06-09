// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

public class SynchronizedRepository<TEntity, TIdentity> : RepositoryDecorator<TEntity, TIdentity>, ISynchronizedObject {

	private readonly SynchronizedObject _innerSyncObject;

	public SynchronizedRepository(IRepository<TEntity, TIdentity> innerRepository, LockRecursionPolicy lockRecursion = LockRecursionPolicy.SupportsRecursion)
		: this(innerRepository, new SynchronizedObject(lockRecursion)) {
	}

	public SynchronizedRepository(IRepository<TEntity, TIdentity> innerRepository, SynchronizedObject innerSyncObject)
		: base(innerRepository) {
		Guard.ArgumentNotNull(innerSyncObject, nameof(innerSyncObject));
		_innerSyncObject = innerSyncObject;
	}

	public ISynchronizedObject ParentSyncObject {
		get => _innerSyncObject.ParentSyncObject;
		set => _innerSyncObject.ParentSyncObject = value;
	}

	public ReaderWriterLockSlim ThreadLock => _innerSyncObject.ThreadLock;

	public IDisposable EnterReadScope() => _innerSyncObject.EnterReadScope();

	public IDisposable EnterWriteScope() => _innerSyncObject.EnterWriteScope();

	public override bool Contains(TIdentity identity) {
		using (EnterReadScope())
			return base.Contains(identity);
	}

	public override async Task<bool> ContainsAsync(TIdentity identity) {
		using (EnterReadScope())
			return await base.ContainsAsync(identity).ContinueOnSameThread();
		;
	}

	public override bool TryGet(TIdentity identity, out TEntity entity) {
		using (EnterReadScope())
			return base.TryGet(identity, out entity);
	}

	public override async Task<(bool, TEntity)> TryGetAsync(TIdentity identity) {
		using (EnterReadScope())
			return await base.TryGetAsync(identity).ContinueOnSameThread();
	}

	public override void Create(TEntity entity) {
		using (EnterWriteScope())
			base.Create(entity);
	}

	public override async Task CreateAsync(TEntity entity) {
		using (EnterWriteScope())
			await base.CreateAsync(entity).ContinueOnSameThread();
	}

	public override void Update(TEntity entity) {
		using (EnterWriteScope())
			base.Update(entity);
	}

	public override async Task UpdateAsync(TEntity entity) {
		using (EnterWriteScope())
			await base.UpdateAsync(entity).ContinueOnSameThread();
		;
	}

	public override void Delete(TIdentity identity) {
		using (EnterWriteScope())
			base.Delete(identity);
	}

	public override async Task DeleteAsync(TIdentity identity) {
		using (EnterWriteScope())
			await base.DeleteAsync(identity).ContinueOnSameThread();
	}

	public override void Clear() {
		using (EnterWriteScope())
			base.Clear();
	}

	public override async Task ClearAsync() {
		using (EnterWriteScope())
			await base.ClearAsync().ContinueOnSameThread();
		;
	}

	public override void Dispose() {
		using (EnterWriteScope())
			InternalCollection.Dispose();
	}

	public override async ValueTask DisposeAsync() {
		using (EnterWriteScope())
			await base.DisposeAsync().ContinueOnSameThread();
		;
	}
}
