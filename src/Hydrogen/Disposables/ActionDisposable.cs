//-----------------------------------------------------------------------
// <copyright file="ActionScope.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Hydrogen;

public sealed class ActionDisposable : SyncDisposable {
	private readonly Action _endAction;

	public ActionDisposable(Action endAction) {
		_endAction = endAction;
	}

	protected override void FreeManagedResources() {
		_endAction?.Invoke();
	}
}

