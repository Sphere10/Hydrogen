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

public sealed class SyncContextScope : SyncContextScopeBase<SyncContextScope> {
	private readonly Action _rootScopeFinalizer;
	private readonly Action _thisScopeFinalizer;
	private readonly bool _invokeOnException;

	public SyncContextScope(Action rootScopeFinalizer, Action thisScopeFinalizer, ContextScopePolicy policy, string contextName, bool invokeOnException = true) : base(policy, contextName) {
		_rootScopeFinalizer = rootScopeFinalizer;
		_thisScopeFinalizer = thisScopeFinalizer;
		_invokeOnException = invokeOnException;
	}

	protected override void OnScopeEnd(SyncContextScope rootScope, bool inException) {
		if (!inException || inException && _invokeOnException) {
			_thisScopeFinalizer?.Invoke();
			if (IsRootScope)
				_rootScopeFinalizer?.Invoke();
		}
	}
}