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

/// <summary>
/// A ContextScope that implements both synchronous and asynchronous finalization.
/// </summary>
public sealed class ContextScope : ContextScopeBase<ContextScope> {
	private readonly Func<Task> _asyncRootScopeFinalizer;
	private readonly Func<Task> _asyncThisScopeFinalizer;
	private readonly Action _syncRootScopeFinalizer;
	private readonly Action _syncThisScopeFinalizer;
	private readonly bool _invokeOnException;
	public ContextScope(Action syncRootScopeFinalizer, Func<Task> asyncRootScopeFinalizer, Action syncThisScopeFinalizer,
	                        Func<Task> asyncThisFinalizer, ContextScopePolicy policy, string contextID,
	                        bool invokeOnException = true) : base(policy, contextID) {
		_asyncRootScopeFinalizer = asyncRootScopeFinalizer;
		_asyncThisScopeFinalizer = asyncThisFinalizer;
		_syncRootScopeFinalizer = syncRootScopeFinalizer;
		_syncThisScopeFinalizer = syncThisScopeFinalizer;
		_invokeOnException = invokeOnException;
	}
	
	protected override void OnScopeEnd(ContextScope rootScope, bool inException) {
		if (!inException || inException && _invokeOnException) {
			_syncThisScopeFinalizer?.Invoke();
			if (base.IsRootScope)
				_syncRootScopeFinalizer?.Invoke();
		}
	}

	protected override async ValueTask OnScopeEndAsync(ContextScope rootScope, bool inException) {
		if (!inException || inException && _invokeOnException) {
			if (_asyncThisScopeFinalizer != null)
				await _asyncThisScopeFinalizer();
			if (base.IsRootScope)
				await _asyncRootScopeFinalizer?.Invoke();
		}
	}

}