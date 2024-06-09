// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// An <see cref="ILoadable"/> base imlementation, following from <see cref="LoadableBase"/>, that implements it's asynchronous members using <see cref="Task.Run(Action)"/>.
/// </summary>
public abstract class SyncLoadableBase : LoadableBase {

	protected override Task LoadInternalAsync() => Task.Run(LoadInternal);

	protected sealed override Task OnLoadingAsync() => Task.Run(OnLoading);

	protected sealed override Task OnLoadedAsync() => Task.Run(OnLoaded);
}
