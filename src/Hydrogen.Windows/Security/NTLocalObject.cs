// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Security;

/// <summary>
/// Represents a local object like a local user or local group. 
/// </summary>
public abstract class NTLocalObject : NTObject {
	private string _description;

	public string Description {
		get { return _description; }
		set { _description = value; }
	}

	/// <summary>
	/// Updates local state of object to host.
	/// </summary>
	public abstract void Update();

	/// <summary>
	/// Refreshes state from host.
	/// </summary>
	public abstract void Refresh();

	/// <summary>
	/// Deletes this object from host.
	/// </summary>
	public abstract void Delete();

}
