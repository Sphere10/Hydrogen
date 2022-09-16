using System;
using System.Threading.Tasks;

namespace Hydrogen;

public class Scope : Disposable, IScope {
	public event EventHandlerEx ScopeEnd;

	protected virtual void OnScopeEnd() {
	}

	protected virtual ValueTask OnScopeEndAsync() => new(Task.CompletedTask);

	private void NotifyScopeEnd() {
		OnScopeEnd();
		ScopeEnd?.Invoke();
	}

	private async ValueTask NotifyScopeEndAsync() {
		await OnScopeEndAsync();
		await Task.Run(() => ScopeEnd?.Invoke());
	}

	protected override void FreeManagedResources()
		=> NotifyScopeEnd();


	protected override ValueTask FreeManagedResourcesAsync()
		=> NotifyScopeEndAsync();

}

public class Scope<T> : Scope, IScope<T> {
	public T Item { get; set; }
}
