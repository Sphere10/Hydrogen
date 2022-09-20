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

public class TaskScope : AsyncScopeBase {
	private readonly Func<Task> _scopeFinalizer;

	public TaskScope(Func<Task> scopeFinalizer) {
		_scopeFinalizer = scopeFinalizer;
	}

	protected override async ValueTask OnScopeEndAsync() {
		if (_scopeFinalizer != null)
			await _scopeFinalizer();
	}
}

public class TaskScope<T> : TaskScope, IScope<T> {

	public TaskScope(T item, Func<Task> scopeFinalizer)
		: base(scopeFinalizer) {
		Item = item;
	}

	public T Item { get; }
}

