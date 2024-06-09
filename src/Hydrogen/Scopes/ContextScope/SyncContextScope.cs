// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

public abstract class SyncContextScope : ContextScope {

	protected SyncContextScope(ContextScopePolicy policy, string contextID) : base(policy, contextID) {
	}

	protected sealed override async ValueTask OnScopeEndInternalAsync() => await Task.Run(OnScopeEnd);

	protected sealed override async ValueTask OnContextEndAsync() => await Task.Run(OnContextEnd);
}
