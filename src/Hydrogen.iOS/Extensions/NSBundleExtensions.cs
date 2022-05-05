//-----------------------------------------------------------------------
// <copyright file="NSBundleExtensions.cs" company="Sphere 10 Software">
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
using System.IO;
using UIKit;
using Foundation;
using CoreGraphics;
using CoreGraphics;
using Hydrogen;

namespace Hydrogen.iOS
{
	public static class NSBundleExtensions {

	    public static void ExtractFile(this NSBundle bundle, string filename, string destPath, bool overwrite = false) {

	        if (Directory.Exists(destPath))
	            destPath = Path.Combine(destPath, filename);

	        var sourcePath = Path.Combine(bundle.BundlePath, filename);

	        Tools.FileSystem.CopyFile(sourcePath, destPath, overwrite, false);

	    }

	}

}

