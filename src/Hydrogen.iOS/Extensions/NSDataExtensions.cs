//-----------------------------------------------------------------------
// <copyright file="NSDataExtensions.cs" company="Sphere 10 Software">
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
using UIKit;
using Foundation;
using CoreGraphics;
using CoreGraphics;
using Hydrogen;

namespace Hydrogen.iOS
{
	public static class NSDataExtensions {

		public static byte[] ToByteArray(this NSData data) {
			var dataBytes = new byte[(uint)data.Length];
			System.Runtime.InteropServices.Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32((uint)data.Length));
			return dataBytes;
		}

	}

}

