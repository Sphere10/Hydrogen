// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.IO;

namespace Hydrogen.Windows.Security;

public class NTShare : NTLocalObject {
	//_securityDescriptor; 


	internal NTShare() {
	}

	public WinAPI.NETAPI32.SHARE_TYPE Type { get; set; }

	public WinAPI.NETAPI32.GlobalSharePermission Permissions { get; set; }

	public int MaxUses { get; set; }

	public int CurrentUses { get; set; }

	public string ServerPath { get; set; }

	public string Password { get; set; }

	public int Reserved { get; set; }

	public IntPtr SecurityDescriptor { get; set; }

	public override void Update() {
		var shareInfo = new WinAPI.NETAPI32.SHARE_INFO_502 {
			shi502_current_uses = (uint)CurrentUses,
			shi502_max_uses = (uint)MaxUses,
			shi502_netname = Name,
			shi502_passwd = Password,
			shi502_path = ServerPath,
			shi502_permissions = (uint)Permissions,
			shi502_remark = Description,
			shi502_reserved = (uint)Reserved,
			shi502_security_descriptor = SecurityDescriptor,
			shi502_type = (uint)Type
		};


		#region Get security descriptor

		// WinAPI.NETAPI32.SECURITY_DESCRIPTOR secDescriptor;
		//byte[] buffer = new byte[SecurityDescriptor.BinaryLength];
		//SecurityDescriptor.GetBinaryForm(buffer, 0);
		//shareInfo.shi502_security_descriptor = (WinAPI.NETAPI32.SECURITY_DESCRIPTOR)buffer;

		#endregion

		uint parmError = 0;

		var result = WinAPI.NETAPI32.NetShareSetInfo(
			this.NTCompatibleHostName,
			this.Name,
			502,
			ref shareInfo,
			out parmError
		);

		if (result != WinAPI.NETAPI32.NET_API_STATUS.NERR_Success) {
			throw new NetApiException(
				result,
				"Unable to update share '{0}' on host '{1}'.",
				Name,
				Host
			);
		}
	}

	public override void Refresh() {
		var result = TryLoadLocalShare(NTCompatibleHostName, Name, this);

		#region Validation

		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to read share '{0}' on host '{1}'",
				Name,
				Host
			);
		}

		#endregion

	}

	public override void Delete() {
		var result = WinAPI.NETAPI32.NetShareDel(
			this.NTCompatibleHostName,
			this.Name,
			0
		);

		if (result != WinAPI.NETAPI32.NET_API_STATUS.NERR_Success) {
			throw new NetApiException(
				result,
				"Unable to delete share '{0}' on host '{1}'",
				Name,
				Host
			);
		}
	}

	public override string FullName {
		get { return string.Format("{0}{1}{2}", NTCompatibleHostName, Path.DirectorySeparatorChar, Name); }
	}

	public ShareSecurity GetAccessControl() {
		return new ShareSecurity(FullName, AccessControlSections.All);
	}

	public void SetAccessControl(ShareSecurity security) {
		SetAccessControl(FullName, security);
	}

	public static ShareSecurity GetAccessControl(string path) {
		return new ShareSecurity(path, AccessControlSections.All);
	}

	public static void SetAccessControl(string path, ShareSecurity security) {
		security.Persist(path);
	}


	internal static WinAPI.NETAPI32.NET_API_STATUS TryLoadLocalShare(string host, string name, NTShare share) {
		WinAPI.NETAPI32.NET_API_STATUS result;
		var bufPtr = IntPtr.Zero;
		var securityDescriptorPtr = IntPtr.Zero;
		try {
			result = WinAPI.NETAPI32.NetShareGetInfo(host, name, 502, out bufPtr);

			if (result != WinAPI.NETAPI32.NET_API_STATUS.NERR_Success) {
				return result;
			}

			var shareInfo = (WinAPI.NETAPI32.SHARE_INFO_502)Marshal.PtrToStructure(
				bufPtr,
				typeof(WinAPI.NETAPI32.SHARE_INFO_502)
			);

			share.CurrentUses = (int)shareInfo.shi502_current_uses;
			share.Description = shareInfo.shi502_remark;
			share.Host = host;
			share.MaxUses = (int)shareInfo.shi502_max_uses;
			share.Name = shareInfo.shi502_netname;
			share.Password = shareInfo.shi502_passwd;
			share.ServerPath = shareInfo.shi502_path;
			share.Permissions = (WinAPI.NETAPI32.GlobalSharePermission)shareInfo.shi502_permissions;
			share.Reserved = (int)shareInfo.shi502_reserved;
			share.SecurityDescriptor = shareInfo.shi502_security_descriptor;
			share.Description = shareInfo.shi502_remark;

		} finally {
			if (bufPtr != IntPtr.Zero) {
				WinAPI.NETAPI32.NetApiBufferFree(bufPtr);
			}
			if (securityDescriptorPtr != IntPtr.Zero) {
				WinAPI.NETAPI32.NetApiBufferFree(securityDescriptorPtr);
			}
		}
		return result;
	}

}
