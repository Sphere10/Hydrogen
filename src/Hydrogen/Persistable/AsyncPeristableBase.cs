// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// A <see cref="PersistableBase"/> abstraction that implements it's synchronous members using sync-over-async pattern.
/// </summary>
public abstract class AsyncPeristableBase : PersistableBase {

	// Copy of AsyncLoadableBase
	protected override void LoadInternal() => LoadInternalAsync().WaitSafe();

	protected sealed override void OnLoading() => OnLoadingAsync().WaitSafe();

	protected sealed override void OnLoaded() => OnLoadedAsync().WaitSafe();


	// Copy of AsyncSaveableBase
	protected override void SaveInternal() => SaveInternalAsync().WaitSafe();

	protected sealed override void OnSaving() => OnSavingAsync().WaitSafe();

	protected sealed override void OnSaved() => OnSavedAsync().WaitSafe();
}
