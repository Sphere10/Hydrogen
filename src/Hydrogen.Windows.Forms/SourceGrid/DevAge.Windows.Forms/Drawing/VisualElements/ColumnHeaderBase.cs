// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace DevAge.Drawing.VisualElements;

public interface IColumnHeader : IHeader {

}


[Serializable]
public abstract class ColumnHeaderBase : HeaderBase, IColumnHeader {

	#region Constuctor

	/// <summary>
	/// Default constructor
	/// </summary>
	public ColumnHeaderBase() {
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="other"></param>
	public ColumnHeaderBase(ColumnHeaderBase other)
		: base(other) {
	}

	#endregion

}
