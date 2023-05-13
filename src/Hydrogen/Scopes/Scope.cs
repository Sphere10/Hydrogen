// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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