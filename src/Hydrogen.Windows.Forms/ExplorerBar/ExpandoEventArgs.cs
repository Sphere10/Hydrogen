// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// Provides data for the StateChanged, ExpandoAdded and 
/// ExpandoRemoved events
/// </summary>
public class ExpandoEventArgs : EventArgs {

	#region Class Data

	/// <summary>
	/// The Expando that generated the event
	/// </summary>
	private Expando expando;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the ExpandoEventArgs class with default settings
	/// </summary>
	public ExpandoEventArgs() {
		expando = null;
	}


	/// <summary>
	/// Initializes a new instance of the ExpandoEventArgs class with specific Expando
	/// </summary>
	/// <param name="expando">The Expando that generated the event</param>
	public ExpandoEventArgs(Expando expando) {
		this.expando = expando;
	}

	#endregion


	#region Properties

	/// <summary>
	/// Gets the Expando that generated the event
	/// </summary>
	public Expando Expando {
		get { return this.expando; }
	}


	/// <summary>
	/// Gets whether the Expando is collapsed
	/// </summary>
	public bool Collapsed {
		get { return this.expando.Collapsed; }
	}

	#endregion

}
