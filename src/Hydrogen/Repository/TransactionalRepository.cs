// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


namespace Hydrogen;

// TODO: Need to first refactor ITransactionalScope (see Notion for details)

//public class TransactionalRepository<TEntity, TIdentity> : RepositoryDecorator<TEntity, TIdentity>, ITransactionalObject {

//	public event EventHandlerEx<object> Committing { add => _transactionalObject.Committing += value; remove => _transactionalObject.Committing -= value; }
//	public event EventHandlerEx<object> Committed { add => _transactionalObject.Committed += value; remove => _transactionalObject.Committed -= value; }
//	public event EventHandlerEx<object> RollingBack { add => _transactionalObject.RollingBack += value; remove => _transactionalObject.RollingBack -= value; }
//	public event EventHandlerEx<object> RolledBack { add => _transactionalObject.RolledBack += value; remove => _transactionalObject.RolledBack -= value; }

//	private readonly ITransactionalObject _transactionalObject;

//	public TransactionalRepository(IRepository<TEntity, TIdentity> innerRepository, ITransactionalObject transactionalObject) 
//		: base(innerRepository) {
//		Guard.ArgumentNotNull(transactionalObject, nameof(transactionalObject));
//		_transactionalObject = transactionalObject;
//	}

//	public virtual void Commit() => _transactionalObject.Commit();

//	public virtual Task CommitAsync() => _transactionalObject.CommitAsync();

//	public virtual void Rollback() => _transactionalObject.Rollback();

//	public virtual Task RollbackAsync() => _transactionalObject.RollbackAsync();

//	public override void Create(TEntity entity) { 
//		using (this.EnterWriteScope()) 
//			base.Create(entity);
//	}

//	public override async Task CreateAsync(TEntity entity) { 
//		using (EnterWriteScope()) 
//			await base.CreateAsync(entity);
//	}

//	public override void Update(TEntity entity) { 
//		using (EnterWriteScope()) 
//			base.Update(entity);
//	}

//	public override async Task UpdateAsync(TEntity entity) {
//		using (EnterWriteScope()) 
//			await base.UpdateAsync(entity);
//	}

//	public override void Delete(TIdentity identity) { 
//		using (EnterWriteScope()) 
//			base.Delete(identity);
//	}

//	public override Task DeleteAsync(TIdentity identity) { 
//		using (EnterWriteScope()) 
//			return base.DeleteAsync(identity);
//	}

//	public override void Clear() { 
//		using (EnterWriteScope()) 
//			base.Clear();
//	}

//	public override async Task ClearAsync() { 
//		using (EnterWriteScope()) 
//			await base.ClearAsync();
//	}

//	public override void Dispose() => InternalCollection.Dispose();

//	public override async ValueTask DisposeAsync() { 
//		using (EnterWriteScope()) 
//			await base.DisposeAsync();
//	}

//}
