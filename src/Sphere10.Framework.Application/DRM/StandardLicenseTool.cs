//-----------------------------------------------------------------------
// <copyright file="StandardLicenseTool.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sphere10.Framework;


namespace Sphere10.Framework.Application {
	public static class StandardLicenseTool {

		public static byte GetProductMajorVersion(string productVersion) {
			var splits = productVersion.Split(new char[] { '.' });
			byte majorVersion = 0;
			if (splits.Length > 0) {
				if (!byte.TryParse(splits[0], out majorVersion)) {
					throw new SoftwareException("Unable to determine major version of product version {0}", productVersion);
				}
			}
			return majorVersion;
		}
	}
}
