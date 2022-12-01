using System.Threading.Tasks;

namespace Hydrogen;

public class TransactionalScopeDecorator<TTransactionalScope> : ContextScopeDecorator<TTransactionalScope>, ITransactionalScope where TTransactionalScope : ITransactionalScope {
	public event EventHandlerEx<object> Committing { add => Internal.Committing += value; remove => Internal.Committing -= value; }
	public event EventHandlerEx<object> Committed { add => Internal.Committed += value; remove => Internal.Committed -= value; }
	public event EventHandlerEx<object> RollingBack { add => Internal.RollingBack += value; remove => Internal.RollingBack -= value; }
	public event EventHandlerEx<object> RolledBack { add => Internal.RolledBack += value; remove => Internal.RolledBack -= value; }

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