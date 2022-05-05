//-----------------------------------------------------------------------
// <copyright file="UIColorExtensions.cs" company="Sphere 10 Software">
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
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace Hydrogen.iOS {
    public static class UIColorExtensions {

        public static Color ToColor(this UIColor uiColor) {
            nfloat alpha, red, green, blue;
            uiColor.GetRGBA(out red, out green, out blue, out alpha);
			
            return Color.FromArgb(
                (int)(alpha * 255.0f),
                (int)(red * 255.0f), 
                (int)(green * 255.0f),
                (int)(blue * 255.0f)
            );
        }
		
        public static UIColor ToUIColor(this Color color) {
            return UIColor.FromRGBA(
                color.R / 255.0f,
                color.G / 255.0f,
                color.B / 255.0f,
                color.A / 255.0f
            );
        }
    }
}
