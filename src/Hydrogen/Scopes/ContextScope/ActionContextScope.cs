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
using System.Threading.Tasks;

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