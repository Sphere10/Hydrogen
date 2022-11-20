using System;
using System.Threading.Tasks;

namespace Hydrogen;

public class SyncOnlyScope : ScopeBase {

	public SyncOnlyScope() {
	}

	protected override void OnScopeEnd() {
	}

	protected sealed override async ValueTask OnScopeEndAsync() => throw new NotSupportedException("This scope does not support async");


}
