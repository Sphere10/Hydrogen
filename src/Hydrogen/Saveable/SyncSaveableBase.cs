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
/// A <see cref="SaveableBase"/> abstraction that implements it's asynchronous members using <see cref="Task.Run(Action)"/>.
/// </summary>
public abstract class SyncSaveableBase : SaveableBase {

	protected override Task SaveInternalAsync() => Task.Run(SaveInternal);

	protected sealed override Task OnSavingAsync() => Task.Run(OnSaving);

	protected sealed override Task OnSavedAsync() => Task.Run(OnSaved);
}
