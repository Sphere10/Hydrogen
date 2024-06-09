// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// A <see cref="LoadableBase"/> abstraction that implements it's synchronous members using sync-over-async pattern.
/// </summary>
public abstract class AsyncLoadableBase : LoadableBase {
	protected override void LoadInternal() => LoadInternalAsync().WaitSafe();

	protected sealed override void OnLoading() => OnLoadingAsync().WaitSafe();

	protected sealed override void OnLoaded() => OnLoadedAsync().WaitSafe();
}
