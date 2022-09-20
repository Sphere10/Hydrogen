namespace Hydrogen;

public abstract class AsyncScopeBase : ScopeBase {

	protected sealed override void OnScopeEnd()
		=> OnScopeEndAsync().AsTask().WaitSafe();
	
}
