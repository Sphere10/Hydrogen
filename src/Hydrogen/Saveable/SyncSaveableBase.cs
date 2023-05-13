// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
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
