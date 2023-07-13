// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Cells.Virtual {
	/// <summary>
	/// A Cell with an Image. Write and read byte[] values.
	/// </summary>
	public class Image : CellVirtual {
		/// <summary>
		/// Constructor using a ValueImage model to read he image directly from the value of the cell.
		/// </summary>
		public Image() {
			Model.AddModel(Models.ValueImage.Default);
			Editor = Editors.ImagePicker.Default;
		}
	}
}

namespace SourceGrid.Cells {
	/// <summary>
	/// A Cell with an Image. Write and read byte[] values.
	/// </summary>
	public class Image : Cell {

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Image() : this(null) {
		}
		/// <summary>
		/// Constructor using a ValueImage model to read he image directly from the value of the cell.
		/// </summary>
		public Image(object value) : base(value) {
			//First I remove the old IImage model that the Cell use to link the Image property to an external value.
			Model.RemoveModel(Model.FindModel(typeof(Models.Image)));

			//Then I add a new IImage model that takes the image directly from the value.
			Model.AddModel(Models.ValueImage.Default);
			Editor = Editors.ImagePicker.Default;
		}

		#endregion

	}
}
