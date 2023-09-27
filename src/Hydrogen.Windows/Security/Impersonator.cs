// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Hydrogen.Windows.Security;

/////////////////////////////////////////////////////////////////////////
/// <summary>
/// Impersonation of a user. Allows to execute code under another
/// user context.
/// Please note that the account that instantiates the Impersonator class
/// needs to have the 'Act as part of operating system' privilege set.
/// </summary>
/// <remarks>	
/// This class is based on the information in the Microsoft knowledge base
/// article http://support.microsoft.com/default.aspx?scid=kb;en-us;Q306158
/// 
/// Encapsulate an instance into a using-directive like e.g.:
///
///		Impersonator.RunAs("myUsername", "myDomainname", "myPassword", action);
///		...
///		using (Impersonator.Scope( ) )
///		{
///			...
///			[code that executes under the new context]
///			...
///		}
///		...
/// 
/// </remarks>
public class Impersonator {

	public static void RunAs(
		string userName,
		string domain,
		string password,
		WinAPI.ADVAPI32.LOGON_TYPE logonType,
		WinAPI.ADVAPI32.LOGON_PROVIDER logonProvider,
		WinAPI.ADVAPI32.SECURITY_IMPERSONATION_LEVEL impersonationLevel,
		bool usePrimaryToken,
		bool attemptEnableSeRestorePrivilege,
		Action code
	) {
		var token = IntPtr.Zero;
		var tokenDuplicate = IntPtr.Zero;

		try {
			if (WinAPI.ADVAPI32.RevertToSelf()) {
				if (WinAPI.ADVAPI32.LogonUser(userName, domain, password, (int)logonType, (int)logonProvider, out token)) {
					if (usePrimaryToken) {
						if (!WinAPI.ADVAPI32.DuplicateTokenEx(token, WinAPI.NETAPI32.MAXIMUM_ALLOWED, impersonationLevel, WinAPI.ADVAPI32.TOKEN_TYPE.TokenPrimary, out tokenDuplicate)) {
							throw new WindowsException(Marshal.GetLastWin32Error(), "Unable to impersonate user '{0}'", userName);
						}
						if (attemptEnableSeRestorePrivilege) {
							if (!Tools.WinTool.ModifyState(tokenDuplicate, "SeRestorePrivilege", true)) {
								//throw new WindowsException(Marshal.GetLastWin32Error(), "Unable to enable SeRestorePrivilege in impersonated security context");
							}
						}

					} else {
						// use an impersonation token via simplified API call
						if (!WinAPI.ADVAPI32.DuplicateToken(token, impersonationLevel, out tokenDuplicate)) {
							throw new WindowsException(Marshal.GetLastWin32Error(), "Unable to impersonate user '{0}'", userName);
						}
					}

					var tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
					WindowsIdentity.RunImpersonated(tempWindowsIdentity.AccessToken, code);

				} else {
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			} else {
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		} finally {
			if (token != IntPtr.Zero) {
				WinAPI.KERNEL32.CloseHandle(token);
			}
			if (tokenDuplicate != IntPtr.Zero) {
				WinAPI.KERNEL32.CloseHandle(tokenDuplicate);
			}
		}
	}

}
