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
	private readonly bool _invokeOnException;

	public ActionContextScope(Action contextFinalizer, ContextScopePolicy policy, string contextName, bool invokeOnException = true)
		: this(contextFinalizer, default, policy, contextName, invokeOnException) {
	}

	public ActionContextScope(Action contextFinalizer, Action scopeFinalizer, ContextScopePolicy policy, string contextName, bool invokeOnException = true) : base(policy, contextName) {
		_contextFinalizer = contextFinalizer;
		_scopeFinalizer = scopeFinalizer;
		_invokeOnException = invokeOnException;
	}

	protected override void OnScopeEndInternal() {
		if (!InException || InException && _invokeOnException && _scopeFinalizer != null)
			_scopeFinalizer();
	}

	protected override void OnContextEnd() {
		if (!InException || InException && _invokeOnException && _contextFinalizer != null)
			_contextFinalizer();
	}

}