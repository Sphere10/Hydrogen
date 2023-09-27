// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Runtime.InteropServices;
using System.Text;

namespace Hydrogen.Windows;

public static partial class WinAPI {

	public static class WTSAPI32 {


		public enum WTS_CONFIG_CLASS {
			WTSUserConfigInitialProgram,
			WTSUserConfigWorkingDirectory,
			WTSUserConfigfInheritInitialProgram,
			WTSUserConfigfAllowLogonTerminalServer,
			WTSUserConfigTimeoutSettingsConnections,
			WTSUserConfigTimeoutSettingsDisconnections,
			WTSUserConfigTimeoutSettingsIdle,
			WTSUserConfigfDeviceClientDrives,
			WTSUserConfigfDeviceClientPrinters,
			WTSUserConfigfDeviceClientDefaultPrinter,
			WTSUserConfigBrokenTimeoutSettings,
			WTSUserConfigReconnectSettings,
			WTSUserConfigModemCallbackSettings,
			WTSUserConfigModemCallbackPhoneNumber,
			WTSUserConfigShadowingSettings,
			WTSUserConfigTerminalServerProfilePath,
			WTSUserConfigTerminalServerHomeDir,
			WTSUserConfigTerminalServerHomeDirDrive,
			WTSUserConfigfTerminalServerRemoteHomeDir
		}


		///// TODO


		[DllImport("Wtsapi32.dll", EntryPoint = "WTSQueryUserConfig")]
		public static extern bool WTSQueryUserConfig(
			[MarshalAs(UnmanagedType.LPTStr)] string pServerName,
			[MarshalAs(UnmanagedType.LPTStr)] string pUserName,
			WTS_CONFIG_CLASS WTSConfigClass,
			StringBuilder ppBuffer,
			out uint pBytesReturned);

		[DllImport("Wtsapi32.dll", EntryPoint = "WTSSetUserConfig")]
		public static extern bool WTSSetUserConfig(
			[MarshalAs(UnmanagedType.LPTStr)] string pServerName,
			[MarshalAs(UnmanagedType.LPTStr)] string pUserName,
			WTS_CONFIG_CLASS WTSConfigClass,
			[MarshalAs(UnmanagedType.LPTStr)] string pBuffer,
			uint dataLength);


	}

}
