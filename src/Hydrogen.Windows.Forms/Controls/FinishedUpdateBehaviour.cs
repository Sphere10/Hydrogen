// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.Forms;

[Flags]
public enum FinishedUpdateBehaviour {
	/// <summary>
	/// Does not raise any event or change any UI/Model state
	/// </summary>
	DoNothing = 0,

	/// <summary>
	/// Raises StateChanged event
	/// </summary>
	NotifyStateChanged = 1 << 0,

	/// <summary>
	/// Copies UI to Model irrepesctive of UpdateModelOnStateChanged value"/>
	/// </summary>
	ForceCopyUIToModel = 1 << 1,

	/// <summary>
	///  Does not call StateChanged Event but copies UI to model
	/// </summary>
	CopyModelToUI = 1 << 2,

	Default = NotifyStateChanged,
}
