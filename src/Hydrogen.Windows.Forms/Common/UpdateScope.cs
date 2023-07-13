// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.Forms;

internal class UpdateScope : IDisposable {
	private readonly FinishedUpdateBehaviour _behaviour;
	private readonly IUpdatable _updateable;

	public UpdateScope(IUpdatable updateable, FinishedUpdateBehaviour behaviour) {
		if (!updateable.Updating) {
			_updateable = updateable;
			_updateable.BeginUpdate();
			_behaviour = behaviour;
		}
	}

	public void Dispose() {
		if (_updateable != null) {
			_updateable.FinishUpdate(_behaviour);
		}
	}
}
