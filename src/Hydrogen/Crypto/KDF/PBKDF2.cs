// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Security.Cryptography;

namespace Hydrogen {

	public static class PBKDF2 {
		public static byte[] DeriveKey(string secret, byte[] salt, int iterations, int keyLength) {
			var pbkdf2 = new Rfc2898DeriveBytes(secret, salt) {
				IterationCount = iterations
			};
			return pbkdf2.GetBytes(keyLength);
		}

	}

}