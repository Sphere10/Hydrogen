//-----------------------------------------------------------------------
// <copyright file="NSCellExtensions.cs" company="Sphere 10 Software">
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
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Hydrogen {
	public static class NSCellExtensions {

		public static int DetermineFitWidth(this NSCell cell, NSObject objectValueAtCell, int minWidth = 10) {
			int width;
			if (cell is NSTextFieldCell) {
				var font = cell.Font ?? NSFont.ControlContentFontOfSize(-1);
				var attrs = NSDictionary.FromObjectAndKey(font, NSAttributedString.FontAttributeName);
				
				// Determine the text on the cell
				NSString cellText;
				if (objectValueAtCell is NSString) {
					cellText = (NSString)objectValueAtCell;
				} else if (cell.Formatter != null) {
					cellText = cell.Formatter.StringFor(objectValueAtCell).ToNSString();
				} else {
					cellText = objectValueAtCell.Description.ToNSString();
				}
				
				width = (int)cellText.StringSize(attrs).Width + minWidth;
				
			} else if (cell.Image != null) {
				// if cell has an image, use that images width
				width = (int)Math.Max(minWidth, (int)cell.Image.Size.Width);
			}  else {
				// cell is something else, just use its width
				width = (int)Math.Max(minWidth, (int)cell.CellSize.Width);
			}

			return width;

		}
	
	}
}

