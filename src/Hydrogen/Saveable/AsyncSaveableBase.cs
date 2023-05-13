// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// A <see cref="SaveableBase"/> abstraction that implements it's synchronous members using sync-over-async pattern.
/// </summary>
public abstract class AsyncSaveableBase : SaveableBase {
	
	protected override void SaveInternal() => SaveInternalAsync().WaitSafe();

	protected sealed override void OnSaving() => OnSavingAsync().WaitSafe();

	protected sealed override void OnSaved() => OnSavedAsync().WaitSafe();
}
