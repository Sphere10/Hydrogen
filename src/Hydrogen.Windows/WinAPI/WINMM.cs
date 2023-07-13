// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.InteropServices;

namespace Hydrogen.Windows;

public static partial class WinAPI {

	public static class WINMM {
		[DllImport("winmm.dll", SetLastError = true)]
		public static extern bool PlaySound(string pszSound,
		                                    System.UIntPtr hmod, uint fdwSound);

		[DllImport("winmm.dll", SetLastError = true)]
		public static extern bool PlaySound(byte[] ptrToSound,
		                                    System.UIntPtr hmod, uint fdwSound);

		[DllImport("winmm.dll", SetLastError = true)]
		public static extern bool PlaySound(IntPtr ptrToSound,
		                                    System.UIntPtr hmod, uint fdwSound);


		[Flags]
		public enum SoundFlags : int {
			SND_SYNC = 0x0000, // play synchronously (default)
			SND_ASYNC = 0x0001, // play asynchronously
			SND_NODEFAULT = 0x0002, // silence (!default) if sound not found
			SND_MEMORY = 0x0004, // pszSound points to a memory file
			SND_LOOP = 0x0008, // loop the sound until next sndPlaySound
			SND_NOSTOP = 0x0010, // don't stop any currently playing sound
			SND_NOWAIT = 0x00002000, // don't wait if the driver is busy
			SND_ALIAS = 0x00010000, // name is a registry alias
			SND_ALIAS_ID = 0x00110000, // alias is a predefined id
			SND_FILENAME = 0x00020000, // name is file name
		}
	}

}
