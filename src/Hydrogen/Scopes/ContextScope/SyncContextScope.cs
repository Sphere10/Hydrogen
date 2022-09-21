using System.Threading.Tasks;

namespace Hydrogen;

public abstract class SyncContextScope : ContextScope {

	protected SyncContextScope(ContextScopePolicy policy, string contextID) : base(policy, contextID) {
	}

	protected sealed override async ValueTask OnScopeEndInternalAsync() => await Task.Run(OnScopeEnd);

	protected sealed override async ValueTask OnContextEndAsync() => await Task.Run(OnContextEnd);
}
