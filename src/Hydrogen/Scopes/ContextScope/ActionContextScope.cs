// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ActionContextScope : SyncContextScope {
	private readonly Action _contextFinalizer;
	private readonly Action _scopeFinalizer;

	public ActionContextScope(Action contextFinalizer, ContextScopePolicy policy, string contextName)
		: this(contextFinalizer, default, policy, contextName) {
	}

	public ActionContextScope(Action contextFinalizer, Action scopeFinalizer, ContextScopePolicy policy, string contextName) : base(policy, contextName) {
		_contextFinalizer = contextFinalizer;
		_scopeFinalizer = scopeFinalizer;
	}

	protected override void OnScopeEndInternal() {
		if (_scopeFinalizer != null)
			_scopeFinalizer();
	}

	protected override void OnContextEnd() {
		if (_contextFinalizer != null)
			_contextFinalizer();
	}

}
