// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen;

namespace Hydrogen;

public abstract class AsyncContextScope : ContextScope {
	protected AsyncContextScope(ContextScopePolicy policy, string contextID) : base(policy, contextID) {
	}

	protected sealed override void OnScopeEndInternal() => OnScopeEndAsync().AsTask().WaitSafe();

	protected sealed override void OnContextEnd() => OnContextEndAsync().AsTask().WaitSafe();

}
