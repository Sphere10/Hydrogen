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

	public static class OLE32 {


		[DllImport("ole32.dll")]
		public static extern int CoInitializeSecurity(IntPtr pVoid, int cAuthSvc, IntPtr asAuthSvc, IntPtr pReserved1, RpcAuthnLevel level, RpcImpLevel impers, IntPtr pAuthList, EoAuthnCap dwCapabilities, IntPtr pReserved3);


		//[Flags]
		// REMOVED BY H.S 2015-02-11
		public enum RpcAuthnLevel {
			Default = 0,
			None = 1,
			Connect = 2,
			Call = 3,
			Pkt = 4,
			PktIntegrity = 5,
			PktPrivacy = 6
		}


		public enum RpcImpLevel {
			Default = 0,
			Anonymous = 1,
			Identify = 2,
			Impersonate = 3,
			Delegate = 4
		}


		public enum EoAuthnCap {
			None = 0x00,
			MutualAuth = 0x01,
			StaticCloaking = 0x20,
			DynamicCloaking = 0x40,
			AnyAuthority = 0x80,
			MakeFullSIC = 0x100,
			Default = 0x800,
			SecureRefs = 0x02,
			AccessControl = 0x04,
			AppID = 0x08,
			Dynamic = 0x10,
			RequireFullSIC = 0x200,
			AutoImpersonate = 0x400,
			NoCustomMarshal = 0x2000,
			DisableAAA = 0x1000
		}
	}

}
