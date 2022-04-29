//-----------------------------------------------------------------------
// <copyright file="TaskItemDesigner.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms.Design;

namespace Sphere10.Framework.Windows.Forms {
	

	/// <summary>
	/// A custom designer used by TaskItems to remove unwanted 
	/// properties from the Property window in the designer
	/// </summary>
	internal class TaskItemDesigner : ControlDesigner {
		/// <summary>
		/// Initializes a new instance of the TaskItemDesigner class
		/// </summary>
		public TaskItemDesigner() {

		}


		/// <summary>
		/// Adjusts the set of properties the component exposes through 
		/// a TypeDescriptor
		/// </summary>
		/// <param name="properties">An IDictionary containing the properties 
		/// for the class of the component</param>
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);

			properties.Remove("BackgroundImage");
			properties.Remove("Cursor");
			properties.Remove("ForeColor");
			properties.Remove("FlatStyle");
		}
	}

	
}
