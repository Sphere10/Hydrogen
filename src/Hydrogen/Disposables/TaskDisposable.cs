// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;

namespace Hydrogen;

public sealed class TaskDisposable : AsyncDisposable {
	private readonly Func<Task> _scopeFinalizer;

	public TaskDisposable(Func<Task> scopeFinalizer) {
		_scopeFinalizer = scopeFinalizer;
	}

	protected override async ValueTask FreeManagedResourcesAsync() {
		if (_scopeFinalizer != null)
			await _scopeFinalizer();

	}
}
