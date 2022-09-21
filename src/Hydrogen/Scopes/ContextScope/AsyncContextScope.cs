using Hydrogen;

namespace Hydrogen;

public abstract class AsyncContextScope : ContextScope {
	protected AsyncContextScope(ContextScopePolicy policy, string contextID) : base(policy, contextID) {
	}

	protected sealed override void OnScopeEndInternal() => OnScopeEndAsync().AsTask().WaitSafe();

	protected sealed override void OnContextEnd() => OnContextEndAsync().AsTask().WaitSafe();

}
