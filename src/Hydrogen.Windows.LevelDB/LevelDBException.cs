// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.InteropServices;

namespace Hydrogen.Windows.LevelDB;

public class LevelDBException : Exception {
	public LevelDBException(string message) : base(message) {
	}

	public static void Check(IntPtr error) {
		if (error != IntPtr.Zero) {
			try {
				var message = Marshal.PtrToStringAnsi(error);
				throw new LevelDBException(message);
			} finally {
				LevelDBInterop.leveldb_free(error);
			}
		}
	}
}
