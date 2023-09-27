// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Cells.Virtual {
	/// <summary>
	/// A cell that contains a HTML style link. Use the click event to execute the link
	/// </summary>
	public abstract class Link : CellVirtual {
		/// <summary>
		/// Constructor, using VisualModels.Common.LinkStyle and BehaviorModels.Cursor.Default
		/// </summary>
		public Link() {
			View = Views.Link.Default;
			AddController(Controllers.MouseCursor.Hand);
		}
	}
}

namespace SourceGrid.Cells {
	/// <summary>
	/// A cell that contains a HTML style link. Use the click event to execute the link
	/// </summary>
	public class Link : Cell {
		/// <summary>
		/// Constructor
		/// </summary>
		public Link() : this(null) {
		}

		/// <summary>
		/// Constructor using VisualModels.Common.LinkStyle and BehaviorModels.Cursor.Default
		/// </summary>
		/// <param name="p_Value"></param>
		public Link(object p_Value) : base(p_Value) {
			View = Views.Link.Default;
			AddController(Controllers.MouseCursor.Hand);
		}
	}
}
