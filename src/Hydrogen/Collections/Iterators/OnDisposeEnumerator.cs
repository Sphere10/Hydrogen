// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public sealed class OnDisposeEnumerator<T> : EnumeratorDecorator<T> {
	private readonly Action _disposeAction;

	public OnDisposeEnumerator(IEnumerator<T> enumerator, Action disposeAction)
		: base(enumerator) {
		_disposeAction = disposeAction;
	}

	public override void Dispose() {
		_disposeAction?.Invoke();
		base.Dispose();
	}
}
