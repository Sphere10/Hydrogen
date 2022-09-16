using System.Threading.Tasks;

namespace Hydrogen;

public abstract class AsyncContextScopeBase<TScope> :  ContextScopeBase<TScope> where TScope : ContextScopeBase<TScope> {

	protected AsyncContextScopeBase(ContextScopePolicy policy, string contextID) : base(policy, contextID) {
	}

	protected sealed override void OnScopeEnd(TScope rootScope, bool inException) => Task.Run(OnScopeEndAsync);

}
