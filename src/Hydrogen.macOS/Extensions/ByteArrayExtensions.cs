//-----------------------------------------------------------------------
// <copyright file="ByteArrayExtensions.cs" company="Sphere 10 Software">
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
using System.Drawing;
using MonoMac.CoreGraphics;
using System.IO;

namespace Hydrogen {
	public static class ByteArrayExtensions {

		public static NSImage ToNSImage(this byte[] data) {
			return NSImage.FromStream(new MemoryStream(data, false));
		}

	}
}

