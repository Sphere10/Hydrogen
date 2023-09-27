// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace SourceGrid.Cells.Views;

/// <summary>
/// Summary description for VisualModelCheckBox.
/// </summary>
[Serializable]
public class MultiImages : Cell {

	#region Constructors

	/// <summary>
	/// Use default setting
	/// </summary>
	public MultiImages() {
		ElementsDrawMode = DevAge.Drawing.ElementsDrawMode.Covering;
	}

	/// <summary>
	/// Copy constructor.  This method duplicate all the reference field (Image, Font, StringFormat) creating a new instance.
	/// </summary>
	/// <param name="other"></param>
	public MultiImages(MultiImages other)
		: base(other) {
		mImages = (DevAge.Drawing.VisualElements.VisualElementList)other.mImages.Clone();
	}

	#endregion

	private DevAge.Drawing.VisualElements.VisualElementList mImages = new DevAge.Drawing.VisualElements.VisualElementList();

	/// <summary>
	/// Images of the cells
	/// </summary>
	public DevAge.Drawing.VisualElements.VisualElementList SubImages {
		get { return mImages; }
	}

	protected override IEnumerable<DevAge.Drawing.VisualElements.IVisualElement> GetElements() {
		foreach (DevAge.Drawing.VisualElements.IVisualElement v in GetBaseElements())
			yield return v;

		foreach (DevAge.Drawing.VisualElements.IVisualElement v in SubImages)
			yield return v;
	}
	private IEnumerable<DevAge.Drawing.VisualElements.IVisualElement> GetBaseElements() {
		return base.GetElements();
	}

	#region Clone

	/// <summary>
	/// Clone this object. This method duplicate all the reference field (Image, Font, StringFormat) creating a new instance.
	/// </summary>
	/// <returns></returns>
	public override object Clone() {
		return new MultiImages(this);
	}

	#endregion

}
