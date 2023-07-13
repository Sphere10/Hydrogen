// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.AccessControl;

namespace Hydrogen.Windows.Security;

public class SecurityTool {
	public static readonly IActionObserver DefaultActionObserver = new NullActionObserver();


	private class NullActionObserver : IActionObserver {

		#region IActionObserver Members

		public void NotifyAction(string actionName, string objectType, string sourceName, string destName) {
		}

		public void NotifyActionFailed(string action, string objectType, string sourceName, string destName, string reason) {
		}

		public void NotifyInformation(string info, params object[] formatArgs) {
		}

		public void NotifyWarning(string info, params object[] formatArgs) {
		}

		public void NotifyError(string info, params object[] formatArgs) {
		}

		public void NotifyCompleted() {
		}

		#endregion

	}


	/// <summary>
	/// Changes the owner of an object or directory. Make sure SeRestore security is on.
	/// </summary>
	/// <param name="path"></param>
	/// <param name="sid"></param>
	public static void ChangeObjectOwner(string path, SecurityIdentifier sid) {
		var sidPtr = IntPtr.Zero;

		try {
			if (WinAPI.ADVAPI32.ConvertStringSidToSid(sid.ToString(), out sidPtr)) {
				WinAPI.ADVAPI32.SECURITY_DESCRIPTOR secDesc;

				if (!WinAPI.ADVAPI32.InitializeSecurityDescriptor(out secDesc, WinAPI.ADVAPI32.SECURITY_DESCRIPTOR_REVISION)) {
					throw new SoftwareException(
						"Unable to change owner of object '{0}' to SID '{1}'. Applying file/directory security descriptor failed.",
						path,
						sid
					);
				}

				if (!WinAPI.ADVAPI32.SetSecurityDescriptorOwner(ref secDesc, sidPtr, false)) {
					throw new SoftwareException(
						"Unable to change owner of object '{0}' to SID '{1}'. Applying file/directory security descriptor failed.",
						path,
						sid
					);
				}
				if (!WinAPI.ADVAPI32.SetFileSecurity(path, WinAPI.ADVAPI32.OWNER_SECURITY_INFORMATION, ref secDesc)) {
					int error = WinAPI.KERNEL32.GetLastError();
					throw new SoftwareException(
						"Unable to change owner of object '{0}' to SID '{1}'. Applying file/directory security descriptor failed.",
						path,
						sid
					);
				}


			}
		} finally {
			if (sidPtr != IntPtr.Zero) {
				WinAPI.NETAPI32.NetApiBufferFree(sidPtr);
			}
		}


	}


	public static string GetUserFromDomainUserString(string domainUser) {
		string[] splits = domainUser.Split('\\');
		Debug.Assert(splits.Length > 0);
		if (splits.Length == 1) {
			return domainUser;
		}
		;
		return splits[1];
	}

	public static string GetDomainFromDomainUserString(string domainUser) {
		string[] splits = domainUser.Split('\\');
		Debug.Assert(splits.Length > 0);
		if (splits.Length == 1) {
			return domainUser;
		}
		;
		return splits[0];
	}


	public static RawSecurityDescriptor ConvertSecurityDescriptor(WinAPI.ADVAPI32.SECURITY_DESCRIPTOR descriptor) {
		throw new NotImplementedException();
	}


}
