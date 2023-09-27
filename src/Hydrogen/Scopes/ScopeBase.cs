// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
