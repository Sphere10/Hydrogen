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

	public static class COMCTL32 {

		/// <summary>
		/// Receives dynamic-link library (DLL)-specific version information. 
		/// It is used with the DllGetVersion function
		/// </summary>
		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct DLLVERSIONINFO {
			/// <summary>
			/// Size of the structure, in bytes. This member must be filled 
			/// in before calling the function
			/// </summary>
			public int cbSize;

			/// <summary>
			/// Major version of the DLL. If the DLL's version is 4.0.950, 
			/// this value will be 4
			/// </summary>
			public int dwMajorVersion;

			/// <summary>
			/// Minor version of the DLL. If the DLL's version is 4.0.950, 
			/// this value will be 0
			/// </summary>
			public int dwMinorVersion;

			/// <summary>
			/// Build number of the DLL. If the DLL's version is 4.0.950, 
			/// this value will be 950
			/// </summary>
			public int dwBuildNumber;

			/// <summary>
			/// Identifies the platform for which the DLL was built
			/// </summary>
			public int dwPlatformID;
		}


		[DllImport("comctl32.dll", SetLastError = true)]
		public static extern int DllGetVersion(ref DLLVERSIONINFO pdvi);

	}

}
