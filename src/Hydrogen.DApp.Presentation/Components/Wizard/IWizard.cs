// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Presentation.Components.Wizard;

/// <summary>
/// Wizard!
/// </summary>
public interface IWizard<TModel> : IWizard {
	TModel Model { get; }
}


public interface IWizard {
	string Title { get; }

	Type CurrentStep { get; }

	bool HasNext { get; }

	bool HasPrevious { get; }

	void UpdateSteps(StepUpdateType updateType, IEnumerable<Type> steps);

	Result<bool> Next();

	Result<bool> Previous();

	Task<Result<bool>> FinishAsync();

	Task<Result<bool>> CancelAsync();
}
