// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32;
using Hydrogen;
using Hydrogen.Windows;

namespace Tools;

public class WinTool {

	#region Services

	public static bool IsServiceInstalled(string serviceName)
		=> TryGetServiceController(serviceName, out _);

	public static bool TryGetServiceController(string serviceName, out ServiceController serviceController) {
		serviceController = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);
		return serviceController != null;
	}

	#endregion

	#region Registry

	public static bool TryResolveHive(string hiveName, out RegistryHive hive) {
		Debug.Assert(!String.IsNullOrEmpty(hiveName));
		bool retval = true;
		hive = RegistryHive.ClassesRoot;
		switch (hiveName.ToUpper()) {
			case "HKEY_CLASSES_ROOT":
				hive = RegistryHive.ClassesRoot;
				break;
			case "HKEY_CURRENT_USER":
				hive = RegistryHive.CurrentUser;
				break;
			case "HKEY_LOCAL_MACHINE":
				hive = RegistryHive.LocalMachine;
				break;
			case "HKEY_USERS":
				hive = RegistryHive.Users;
				break;
			case "HKEY_CURRENT_CONFIG":
				hive = RegistryHive.CurrentConfig;
				break;
			default:
				retval = false;
				break;
		}
		return retval;
	}

	static public RegistryKey OpenRegistryHive(string hostname, RegistryHive hive) {
		RegistryKey retval = Registry.LocalMachine;
		if (String.IsNullOrEmpty(hostname)) {
			switch (hive) {
				case RegistryHive.ClassesRoot:
					retval = Registry.ClassesRoot;
					break;
				case RegistryHive.CurrentConfig:
					retval = Registry.CurrentConfig;
					break;
				case RegistryHive.CurrentUser:
					retval = Registry.CurrentUser;
					break;
				case RegistryHive.LocalMachine:
					retval = Registry.LocalMachine;
					break;
				case RegistryHive.PerformanceData:
					retval = Registry.PerformanceData;
					break;
				case RegistryHive.Users:
					retval = Registry.Users;
					break;
				default:
					throw new SoftwareException($"Unable to open registry hive '{hive}'");
			}
		} else {
			retval = RegistryKey.OpenRemoteBaseKey(hive, hostname);
		}
		return retval;
	}

	static public bool TryOpenKey(string host, string address, out RegistryKey outputKey) {
		Debug.Assert(!String.IsNullOrEmpty(address));
		outputKey = null;
		bool retval = false;
		try {
			string[] splits = address.Split('\\');
			string hiveText = splits[0].ToUpper();
			RegistryHive hive;
			if (!String.IsNullOrEmpty(splits[0]) && TryResolveHive(splits[0], out hive)) {
				outputKey = OpenRegistryHive(host, hive);
				if (splits.Length > 0) {
					StringBuilder remainingAddress = new StringBuilder();
					for (int i = 1; i < splits.Length; i++) {
						if (i > 1) {
							remainingAddress.Append('\\');
						}
						remainingAddress.Append(splits[i]);
					}
					outputKey = outputKey.OpenSubKey(remainingAddress.ToString(), true);
					retval = true;
				} else {
					retval = true;
				}
			}
		} catch {
		}
		return retval;
	}

	static public RegistryKey OpenKey(string host, string address) {
		Debug.Assert(!String.IsNullOrEmpty(address));
		RegistryKey retval;
		if (!TryOpenKey(host, address, out retval)) {
			throw new SoftwareException(
				String.Format("Could not retrieve the registry key '{0}' as it does not exist", address)
			);
		}
		return retval;
	}

	static public bool KeyExists(string host, string address) {
		RegistryKey key;
		return TryOpenKey(host, address, out key);
	}

	static public string[] GetSubKeys(string host, string key) {

		Debug.Assert(KeyExists(host, key));
		List<string> subKeys = new List<string>();

		string[] subKeyNames = OpenKey(host, key).GetSubKeyNames();

		foreach (string subKeyName in subKeyNames) {
			subKeys.Add(key + "\\" + subKeyName);
		}
		return subKeys.ToArray();
	}

	#endregion

	#region Security

	public static bool EnablePrivilege(string privilege) {
		return ModifyState(privilege, true);
	}

	public static WinAPI.ADVAPI32.LUID LookupPrivilegeID(string name) {
		WinAPI.ADVAPI32.LUID privilegeId;
		if (!WinAPI.ADVAPI32.LookupPrivilegeValue(null, name, out privilegeId)) {
			throw new WindowsException(WinAPI.KERNEL32.GetLastError(), "Unable to retrieve privilege '{0}'", name);
		}
		return privilegeId;
	}

	public static bool DisablePrivilege(string privilege) {
		return ModifyState(privilege, false);
	}

	public static bool ModifyState(string privilege, bool enable) {

		IntPtr processToken = WinAPI.KERNEL32.GetCurrentProcess();
		IntPtr hToken = IntPtr.Zero;
		try {
			if (!WinAPI.ADVAPI32.OpenProcessToken(
				    processToken,
				    WinAPI.NETAPI32.TOKEN_ADJUST_PRIVILEGES,
				    out hToken
			    )) {
				return false;
			}
			return ModifyState(hToken, LookupPrivilegeID(privilege), enable);
		} finally {
			if (hToken != IntPtr.Zero) {
				WinAPI.KERNEL32.CloseHandle(hToken);
			}
		}
	}

	public static bool ModifyState(IntPtr token, string privilege, bool enable) {
		return ModifyState(token, LookupPrivilegeID(privilege), enable);
	}

	public static bool ModifyState(IntPtr token, WinAPI.ADVAPI32.LUID privilegeID, bool enable) {
		bool retval = false;
		WinAPI.ADVAPI32.TOKEN_PRIVILEGES privilege;
		privilege.PrivilegeCount = 1;
		privilege.Privileges = new WinAPI.ADVAPI32.LUID_AND_ATTRIBUTES[privilege.PrivilegeCount];
		privilege.Privileges[0].Luid = privilegeID;
		if (enable) {
			privilege.Privileges[0].Attributes = WinAPI.NETAPI32.SE_PRIVILEGE_ENABLED;
		} else {
			privilege.Privileges[0].Attributes = 0;
		}
		uint retLengthInBytes;
		if (WinAPI.ADVAPI32.AdjustTokenPrivileges(token, false, ref privilege, 1024, out retLengthInBytes)) {
			retval = true;
		}
		return retval;
	}

	#endregion

	#region Misc

	public static ushort HIWORD(IntPtr dwValue) {
		unchecked {
			return (ushort)((((long)dwValue) >> 0x10) & 0xffff);
		}
	}

	public static ushort HIWORD(uint dwValue) {
		unchecked {
			return (ushort)(dwValue >> 0x10);
		}
	}

	public static int GET_WHEEL_DELTA_WPARAM(IntPtr wParam) {
		unchecked {
			return (short)HIWORD(wParam);
		}
	}

	public static int GET_WHEEL_DELTA_WPARAM(uint wParam) {
		unchecked {
			return (short)HIWORD(wParam);
		}
	}

	public static int GET_WHEEL_DELTA_WPARAM(int wParam) {
		unchecked {
			return (short)HIWORD((uint)wParam);
		}
	}

	public static Key VirtualKeyToKey(VirtualKey virtualKey) {
		return (Key)System.Enum.Parse(typeof(Key), virtualKey.ToString());
	}

	#endregion

}
