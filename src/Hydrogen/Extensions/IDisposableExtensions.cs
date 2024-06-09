// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public static class IDisposableExtensions {

	public static IDisposable OnDisposing(this IDisposable disposable, Action disposingAction) 
		=> new ActionDisposable(() => {
			disposingAction();
			disposable.Dispose();
		});

	public static IDisposable OnDisposed(this IDisposable disposable, Action disposedAction) 
		=> new ActionDisposable(() => {
			disposable.Dispose();
			disposedAction();
		});

}
