//-----------------------------------------------------------------------
// <copyright file="CryptoTool.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using Sphere10.Framework;

// ReSharper disable CheckNamespace

namespace Sphere10.Framework {

	public class CryptographicallySecureGUIDGenerator {


		public static Guid NewCryptographicGuid() {

			// byte indices
			int versionByteIndex = BitConverter.IsLittleEndian ? 7 : 6;
			const int variantByteIndex = 8;

			// version mask & shift for `Version 4`
			const int versionMask = 0x0F;
			const int versionShift = 0x40;

			// variant mask & shift for `RFC 4122`
			const int variantMask = 0x3F;
			const int variantShift = 0x80;

			// get bytes of cryptographically-strong random values
			var bytes = Tools.Crypto.GenerateCryptographicallyRandomBytes(16);

			// Set version bits -- 6th or 7th byte according to Endianness, big or little Endian respectively
			bytes[versionByteIndex] = (byte)(bytes[versionByteIndex] & versionMask | versionShift);

			// Set variant bits -- 9th byte
			bytes[variantByteIndex] = (byte)(bytes[variantByteIndex] & variantMask | variantShift);

			// Initialize Guid from the modified random bytes
			return new Guid(bytes);
		}
	}
}