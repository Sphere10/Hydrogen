namespace Hydrogen;

public abstract class AsyncScope : ScopeBase {

	protected sealed override void OnScopeEnd()
		=> OnScopeEndAsync().AsTask().WaitSafe();
	
}
