//-----------------------------------------------------------------------
// <copyright file="NSFontExtensions.cs" company="Sphere 10 Software">
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
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Drawing;


namespace Hydrogen {


	public static class NSFontExtensions
	{
		public static NSFont ToNSFont(this FontStyle fontStyle, string fontFamilyName = "Verdana", int weight = 0, float? size = null) {
			if (!size.HasValue) {
				size = NSFont.LabelFontSize;
			}
			return NSFontManager.SharedFontManager.FontWithFamily(
				fontFamilyName,
				fontStyle.ToNSFontTraitMask(),
				weight,
				size.Value
			);
		}


		public static NSFontTraitMask ToNSFontTraitMask(this FontStyle fontStyle) {
			NSFontTraitMask mask = (NSFontTraitMask)0;

			if (fontStyle.HasFlag(FontStyle.Bold)) {
				mask = mask.SetFlags(NSFontTraitMask.Bold);
			}

			if (fontStyle.HasFlag(FontStyle.Italic)) {
				mask = mask.SetFlags(NSFontTraitMask.Italic);
			}

			if (fontStyle.HasFlag(FontStyle.Regular)) {
			}

			if (fontStyle.HasFlag(FontStyle.Strikeout)) {
			}

			if (fontStyle.HasFlag(FontStyle.Underline)) {
			}

			return mask;

		}

	}
}

