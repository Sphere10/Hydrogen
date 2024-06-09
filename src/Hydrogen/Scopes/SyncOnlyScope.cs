// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
