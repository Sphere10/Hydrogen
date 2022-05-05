//-----------------------------------------------------------------------
// <copyright file="SizeExtensions.cs" company="Sphere 10 Software">
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

using System.Drawing;
using CoreGraphics;

namespace Hydrogen.iOS {

    public static class SizeExtensions {

        public static CGSize ToCGSize(this Size size) {
            return new CGSize(size.Height, size.Height);
        }

    }
}
