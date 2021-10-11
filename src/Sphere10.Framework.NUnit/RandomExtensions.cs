//-----------------------------------------------------------------------
// <copyright file="RandomExtensions.cs" company="Sphere 10 Software">
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


namespace Sphere10.Framework.NUnit {
	public static class RandomExtensions {
		public static object[][] NextObjects(this Random rng, int n, int m) {
			var result = new object[m][];
			for (var i = 0; i < m; i++) {
				result[i] = new object[n];
				for (var j = 0; j < n; j++) {
					result[i][j] = rng.NextString(rng.Next(0, 10));
				}
			}
			return result;
		}
	}
}
