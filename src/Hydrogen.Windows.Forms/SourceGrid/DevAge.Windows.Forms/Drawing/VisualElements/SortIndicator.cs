// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using Hydrogen.Windows.Forms;

namespace DevAge.Drawing.VisualElements;

public interface ISortIndicator : IVisualElement {
	HeaderSortStyle SortStyle { get; set; }
}


/// <summary>
/// A class used to draw a generic sort indicator, usually a arrow. Use the SortStyle to customize the sort style (arrow up or arrow down)
/// </summary>
[Serializable]
public class SortIndicator : Icon, ISortIndicator {

	#region Constuctor

	/// <summary>
	/// Default constructor
	/// </summary>
	public SortIndicator() {
		AnchorArea = new AnchorArea(float.NaN, float.NaN, 0, float.NaN, false, true);
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="other"></param>
	public SortIndicator(SortIndicator other)
		: base(other) {
		SortStyle = other.SortStyle;
	}

	#endregion

	/// <summary>
	/// Clone
	/// </summary>
	/// <returns></returns>
	public override object Clone() {
		return new SortIndicator(this);
	}

	#region Properties

	private HeaderSortStyle mHeaderSortStyle = HeaderSortStyle.None;

	[DefaultValue(HeaderSortStyle.None)]
	public virtual HeaderSortStyle SortStyle {
		get { return mHeaderSortStyle; }
		set {
			mHeaderSortStyle = value;
			if (mHeaderSortStyle == HeaderSortStyle.Ascending)
				Value = Resources.IEW_SortUp;
			else if (mHeaderSortStyle == HeaderSortStyle.Descending)
				Value = Resources.IEW_SortDown;
			else
				Value = null;
		}
	}

	#endregion

}
