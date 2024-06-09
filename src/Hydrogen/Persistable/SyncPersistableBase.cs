// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

public abstract class SyncPersistableBase : PersistableBase {

	// Copy of SyncLoadableBase
	protected override Task LoadInternalAsync() => Task.Run(LoadInternal);

	protected sealed override Task OnLoadingAsync() => Task.Run(OnLoading);

	protected sealed override Task OnLoadedAsync() => Task.Run(OnLoaded);


	// Copy of SyncSaveableBase
	protected override Task SaveInternalAsync() => Task.Run(SaveInternal);

	protected sealed override Task OnSavingAsync() => Task.Run(OnSaving);

	protected sealed override Task OnSavedAsync() => Task.Run(OnSaved);
}
