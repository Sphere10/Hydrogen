// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Presentation.Components.Wizard;

/// <summary>
/// Operations for updating wizard steps
/// </summary>
public enum StepUpdateType {
	/// <summary>
	/// Inject the step after the current step, before the next step.
	/// </summary>
	Inject,

	/// <summary>
	/// Replace all steps after current step with the new step
	/// </summary>
	ReplaceAllNext,

	/// <summary>
	/// Replace all steps from the beginning with the new step
	/// </summary>
	ReplaceAll,

	/// <summary>
	/// Removes all steps after the current step matching any of the types in the parameter collection.
	/// </summary>
	RemoveNext
}
