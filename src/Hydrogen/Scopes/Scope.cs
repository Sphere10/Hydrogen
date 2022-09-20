using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Hydrogen;

public class Scope : ScopeBase {

	protected override void OnScopeEnd() {
	}

	protected override async ValueTask OnScopeEndAsync() {
	}

}

public class Scope<T> : Scope, IScope<T> {
	public T Item { get; set; }
}