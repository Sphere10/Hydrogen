// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Cells.Virtual {
	/// <summary>
	/// A cell that rappresent a header of a table, with 3D effect. This cell override IsSelectable to false. Default use VisualModels.VisualModelHeader.Style1
	/// </summary>
	public abstract class Header : CellVirtual {
		/// <summary>
		/// Constructor
		/// </summary>
		public Header() {
			View = Views.Header.Default;
			AddController(Controllers.Unselectable.Default);
			AddController(Controllers.MouseInvalidate.Default);
			ResizeEnabled = true;
		}

		public bool ResizeEnabled {
			get { return FindController(typeof(Controllers.Resizable)) == Controllers.Resizable.ResizeBoth; }
			set {
				if (value == ResizeEnabled)
					return;

				if (value)
					AddController(Controllers.Resizable.ResizeBoth);
				else
					RemoveController(Controllers.Resizable.ResizeBoth);
			}
		}
	}

}

namespace SourceGrid.Cells {
	/// <summary>
	/// A cell that rappresent a header of a table, with 3D effect. This cell override IsSelectable to false. Default use VisualModels.VisualModelHeader.Style1
	/// </summary>
	public class Header : Cell {
		/// <summary>
		/// Constructor
		/// </summary>
		public Header() : this(null) {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cellValue"></param>
		public Header(object cellValue) : base(cellValue) {
			View = Views.Header.Default;
			AddController(Controllers.Unselectable.Default);
			AddController(Controllers.MouseInvalidate.Default);
			ResizeEnabled = true;
		}

		public bool ResizeEnabled {
			get { return FindController(typeof(Controllers.Resizable)) == Controllers.Resizable.ResizeBoth; }
			set {
				if (value == ResizeEnabled)
					return;

				if (value)
					AddController(Controllers.Resizable.ResizeBoth);
				else
					RemoveController(Controllers.Resizable.ResizeBoth);
			}
		}
	}

}
