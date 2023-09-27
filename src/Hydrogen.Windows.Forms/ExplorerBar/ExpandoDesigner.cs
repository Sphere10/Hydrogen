// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Windows.Forms.Design;
using System.Collections;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// A custom designer used by Expandos to remove unwanted 
/// properties from the Property window in the designer
/// </summary>
internal class ExpandoDesigner : ParentControlDesigner {
	/// <summary>
	/// Initializes a new instance of the ExpandoDesigner class
	/// </summary>
	public ExpandoDesigner()
		: base() {

	}


	/// <summary>
	/// Adjusts the set of properties the component exposes through 
	/// a TypeDescriptor
	/// </summary>
	/// <param name="properties">An IDictionary containing the properties 
	/// for the class of the component</param>
	protected override void PreFilterProperties(IDictionary properties) {
		base.PreFilterProperties(properties);

		properties.Remove("BackColor");
		properties.Remove("BackgroundImage");
		properties.Remove("BorderStyle");
		properties.Remove("Cursor");
		properties.Remove("BackgroundImage");
	}
}
