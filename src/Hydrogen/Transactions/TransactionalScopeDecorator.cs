// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

public class TransactionalScopeDecorator<TTransactionalScope> : ContextScopeDecorator<TTransactionalScope>, ITransactionalScope where TTransactionalScope : ITransactionalScope {
	public event EventHandlerEx Committing { add => Internal.Committing += value; remove => Internal.Committing -= value; }
	public event EventHandlerEx Committed { add => Internal.Committed += value; remove => Internal.Committed -= value; }
	public event EventHandlerEx RollingBack { add => Internal.RollingBack += value; remove => Internal.RollingBack -= value; }
	public event EventHandlerEx RolledBack { add => Internal.RolledBack += value; remove => Internal.RolledBack -= value; }

	public TransactionalScopeDecorator(TTransactionalScope internalScope) : base(internalScope) {
	}

	public override IContextScope RootScope => Internal.RootScope;

	ITransactionalScope ITransactionalScope.RootScope => RootScope as ITransactionalScope;

	public virtual void BeginTransaction() => Internal.BeginTransaction();

	public virtual Task BeginTransactionAsync() => Internal.BeginTransactionAsync();

	public virtual void Commit() => Internal.Commit();

	public virtual Task CommitAsync() => Internal.CommitAsync();

	public virtual void Rollback() => Internal.Rollback();

	public virtual Task RollbackAsync() => Internal.RollbackAsync();

}
