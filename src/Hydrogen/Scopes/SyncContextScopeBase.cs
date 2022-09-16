using System.Threading.Tasks;

namespace Hydrogen;

public abstract class SyncContextScopeBase<TScope> :  ContextScopeBase<TScope> where TScope : ContextScopeBase<TScope> {

	protected SyncContextScopeBase(ContextScopePolicy policy, string contextID) : base(policy, contextID) {
	}

	protected sealed override async ValueTask OnScopeEndAsync(TScope rootScope, bool inException) => OnScopeEnd(rootScope, inException);

}
