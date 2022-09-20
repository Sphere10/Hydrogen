using System.Threading.Tasks;

namespace Hydrogen;

public abstract class ScopeBase : Disposable, IScope {
	
	public event EventHandlerEx ScopeEnd;

	protected abstract void OnScopeEnd();

	protected abstract ValueTask OnScopeEndAsync();
	
	protected override void FreeManagedResources()
		=> NotifyScopeEnd();
	
	protected override ValueTask FreeManagedResourcesAsync()
		=> NotifyScopeEndAsync();

	private void NotifyScopeEnd() {
		OnScopeEnd();
		ScopeEnd?.Invoke();
	}

	private async ValueTask NotifyScopeEndAsync() {
		await OnScopeEndAsync();
		await Task.Run(() => ScopeEnd?.Invoke());
	}
}
