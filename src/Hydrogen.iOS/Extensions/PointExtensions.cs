//-----------------------------------------------------------------------
// <copyright file="PointExtensions.cs" company="Sphere 10 Software">
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

namespace Hydrogen.iOS {

    public static class PointExtensions {
        public static CGPoint ToCGPoint(this Point point) {
            return new CGPoint(point.X, point.Y);
        }
    }
}
