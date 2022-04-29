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

using System.IO;
using System.Drawing;

namespace Hydrogen {

	public static class DrawingByteArrayExtensions {

		/// <summary>
		/// Converts the byte array to an Image object.
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static Image ToImage(this byte[] bytes) {
			if (bytes == null || bytes.Length == 0)
				return null;

			using (var mem = new MemoryStream(bytes)) {
				return Image.FromStream(mem);
			}
		}

	}
}
