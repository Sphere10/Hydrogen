// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ActionScope : SyncScope {
	private readonly Action _endAction;

	public ActionScope(Action endAction) {
		_endAction = endAction;
	}

	protected override void OnScopeEnd() {
		_endAction?.Invoke();
	}
}

public class ActionScope<T> : ActionScope, IScope<T> {

	public ActionScope(T item, Action<T> endAction)
		: base(() => endAction(item)) {
		Item = item;
	}

	public T Item { get; }
}

