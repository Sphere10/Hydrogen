// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

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
