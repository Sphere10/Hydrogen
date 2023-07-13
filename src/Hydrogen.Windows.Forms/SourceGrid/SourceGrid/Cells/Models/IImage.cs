// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Cells.Models;

/// <summary>
/// Interface for informations about an image.
/// </summary>
public interface IImage : IModel {
	/// <summary>
	/// Get the image of the specified cell. 
	/// </summary>
	/// <param name="cellContext"></param>
	/// <returns></returns>
	System.Drawing.Image GetImage(CellContext cellContext);
}
