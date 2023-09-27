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
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;


namespace Hydrogen.Windows.Security;

/// <summary>
/// Encapsulates a remote host.
/// </summary>
/// <remarks>
/// Make sure remote host allows anonymous enumeration of users and groups.
/// For Windows 2003/XP/Vista make sure
///    HKLM\System\CurrentControlSet\Control\Lsa\RestrictAnonymousSAM = 1
///    
/// For Windows 2000 make sure
///      HKLM\System\CurrentControlSet\Control\Lsa\RestrictAnonymousSAM = 0
/// 
/// To set it as a policy
///  Computer Configuration/Windows Settings /Security Settings/Local Policies/Security Options/Network Access: Do not allow anonymous enumeration of SAM accounts = Disabled.
///  
/// For more information see http://web.mit.edu/win/security.html
/// </remarks>
public class NTHost : NTObject {
	static private readonly NTHost _currentMachine = new NTHost(null);

	/// <summary>
	/// Constructor establishes connection to host.
	/// </summary>
	/// <param name="hostToConnectTo">Name of host to connect to, or null/empty for local host.</param>
	public NTHost(string host) {
		string hostToConnectTo = string.IsNullOrEmpty(host) ? Environment.MachineName : host;
		IPHostEntry hostEntry = Dns.GetHostEntry(hostToConnectTo);
		Host = hostToConnectTo;
		Aliases = hostEntry.Aliases;
		Addresses = hostEntry.AddressList;
		Name = hostEntry.HostName;

		try {

			#region Get SID for host

			var resolver = new SidNameResolver(this.Host);
			var nameUse = WinAPI.ADVAPI32.SidNameUse.Unknown;
			SecurityIdentifier sid = null;


			// search SID for all aliases & name
			var namesToSearch = new List<string>();
			var resolveErrors = new List<string>();
			namesToSearch.Add(this.Host);
			namesToSearch.AddRange(Aliases);
			namesToSearch.Add(Name);
			var found = false;
			var nameMatched = string.Empty;
			foreach (var name in namesToSearch) {
				string resolveError;
				string resolvedDomain;
				if (resolver.TryReverseResolve(name, out sid, out resolvedDomain, out nameUse, out resolveError)) {
					found = true;
					nameMatched = name;
					break;
				} else {
					resolveErrors.Add(resolveError);
				}
			}
			if (!found) {
				throw new Exception(
					string.Format(
						"Could not establish SID for host '{0}'. Tried the following symbolic aliases '{1}' and each failed with win32 errors '{2}'",
						this.Name,
						string.Join(",", namesToSearch.ToArray()),
						string.Join(",", resolveErrors.ToArray())
					)
				);
			}

			if (nameUse != WinAPI.ADVAPI32.SidNameUse.Computer && nameUse != WinAPI.ADVAPI32.SidNameUse.Domain) {
				throw new Exception(
					string.Format(
						"Host SID resolved to a non-computer and non-domain object. Host was '{0}' and first SID alias matched was '{1}' out of the of list '{2}'.",
						Name,
						nameMatched,
						string.Join(",", namesToSearch.ToArray())
					)
				);
			}
			base.SID = sid;
			base.SidNameUsage = nameUse;

			#endregion

		} catch {
#warning Dodgy workaround - can't get host SID for domain controllers. Ignorning for now
			try {
				SidNameUsage = WinAPI.ADVAPI32.SidNameUse.Invalid;
				SID = new SecurityIdentifier(WellKnownSidType.BuiltinDomainSid, null);
			} catch {
				try {
					SID = new SecurityIdentifier(WellKnownSidType.LocalSid, null);
				} catch {
					SID = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null);
				}
			}
		}
	}

	/// <summary>
	/// Gets the current machine.
	/// </summary>
	public static NTHost CurrentMachine {
		get { return NTHost._currentMachine; }
	}

	public override string FullName {
		get { return Name; }
	}

	public string[] Aliases { get; set; }

	public IPAddress[] Addresses { get; set; }

	public static bool CanConnectTo(string hostname) {
		try {
			var host = new NTHost(hostname);
		} catch {
			return false;
		}
		return true;
	}

	/// <summary>
	/// Returns the local users on the host.
	/// </summary>
	/// <returns></returns>
	public NTLocalGroup[] GetLocalGroups() {
		var bufPtr = IntPtr.Zero;
		var localGroups = new List<NTLocalGroup>();

		try {
			uint entriesRead;
			uint totalEntries;
			int resumeHandle;
			var result = WinAPI.NETAPI32.NetLocalGroupEnum(
				this.NTCompatibleHostName,
				(uint)0,
				out bufPtr,
				(int)-1,
				out entriesRead,
				out totalEntries,
				out resumeHandle
			);
			if (result != 0) {
				throw new NetApiException(
					result,
					"Failed to enumerate local groups on host '{0}'",
					Host
				);
			}

			var iter = bufPtr;
			var structSize = Marshal.SizeOf(typeof(WinAPI.NETAPI32.LOCALGROUP_INFO_0));
			var startAddr = bufPtr.ToInt64();
			var endAddr = startAddr + (int)entriesRead * structSize;
			for (var offset = startAddr; offset < endAddr; offset += structSize) {
				var groupInfo =
					(WinAPI.NETAPI32.LOCALGROUP_INFO_0)Marshal.PtrToStructure(
						new IntPtr(offset),
						typeof(WinAPI.NETAPI32.LOCALGROUP_INFO_0)
					);
				var group = new NTLocalGroup();
				group.Host = Host;
				group.Name = groupInfo.name;
				group.Refresh();
				localGroups.Add(group);
			}
		} finally {
			if (bufPtr != IntPtr.Zero) {
				WinAPI.NETAPI32.NetApiBufferFree(bufPtr);
			}
		}
		return localGroups.ToArray();
	}

	public bool TryGetLocalObject(string remoteName, out NTLocalObject obj) {
		var retval = false;
		obj = null;
		NTLocalUser localUser;
		;
		NTLocalGroup localGroup;
		if (TryGetLocalUser(remoteName, out localUser)) {
			obj = localUser;
			retval = true;
		} else if (TryGetLocalGroup(remoteName, out localGroup)) {
			obj = localGroup;
			retval = true;
		}
		return retval;
	}

	public bool TryGetLocalGroup(string name, out NTLocalGroup group) {
		group = new NTLocalGroup();
		if (NTLocalGroup.TryLoadLocalGroup(NTCompatibleHostName, name, group) == 0) {
			return true;
		} else {
			group = null;
			return false;
		}
	}

	/// <summary>
	/// Creates a local group on the host.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="description"></param>
	public NTLocalGroup CreateLocalGroup(string name, string description) {
		var error = 0;
		var newLocalGroup = new WinAPI.NETAPI32.LOCALGROUP_INFO_1();
		newLocalGroup.lgrpi1_name = name;
		newLocalGroup.lgrpi1_comment = description;
		var result = WinAPI.NETAPI32.NetLocalGroupAdd(this.NTCompatibleHostName, 1, ref newLocalGroup, ref error);
		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to create local group '{0}' on host '{1}'",
				name,
				Host
			);
		}

		NTLocalGroup localGroup = new NTLocalGroup();
		localGroup.Host = Host;
		localGroup.Name = name;
		localGroup.Description = description;
		localGroup.Refresh();

		return localGroup;
	}


	/// <summary>
	/// Returns the local users on the host.
	/// </summary>
	/// <returns></returns>
	public NTLocalUser[] GetLocalUsers() {
		var bufPtr = IntPtr.Zero;
		var localUsers = new List<NTLocalUser>();

		try {
			int entriesRead;
			int totalEntries;
			int resumeHandle;
			var result = WinAPI.NETAPI32.NetUserEnum(
				this.NTCompatibleHostName,
				3,
				2,
				out bufPtr,
				-1,
				out entriesRead,
				out totalEntries,
				out resumeHandle
			);
			if (result != 0) {
				throw new NetApiException(
					result,
					"Failed to enumerate local users on host '{0}'",
					Host
				);
			}
			var structSize = Marshal.SizeOf(typeof(WinAPI.NETAPI32.USER_INFO_3));
			var startAddr = bufPtr.ToInt64();
			var endAddr = startAddr + entriesRead * structSize;
			for (var offset = startAddr; offset < endAddr; offset += structSize) {
				var userInfo =
					(WinAPI.NETAPI32.USER_INFO_3)Marshal.PtrToStructure(
						new IntPtr(offset),
						typeof(WinAPI.NETAPI32.USER_INFO_3)
					);
				var user = new NTLocalUser {
					Host = Host,
					Name = !string.IsNullOrEmpty(userInfo.usri3_name) ? userInfo.usri3_name : string.Empty
				};
				user.Refresh();
				localUsers.Add(user);
			}
		} catch (Exception error) {
#warning Need to fix this
			//throw new ApplicationException(

		} finally {
			if (bufPtr != IntPtr.Zero) {
				WinAPI.NETAPI32.NetApiBufferFree(bufPtr);
			}
		}
		return localUsers.ToArray();
	}

	public bool TryGetLocalUser(string name, out NTLocalUser user) {
		user = new NTLocalUser();
		if (NTLocalUser.TryLoadLocalUser(NTCompatibleHostName, name, user) == 0) {
			return true;
		}
		user = null;
		return false;
	}

	/// <summary>
	/// Creates a local user on the host.
	/// </summary>
	/// <param name="name"></param>
	public NTLocalUser CreateLocalUser(string name, string password) {
		var newUser = new WinAPI.NETAPI32.USER_INFO_1 {
			usri1_name = name,
			usri1_password = password,
			usri1_priv = (int)UserPrivilege.User,
			usri1_home_dir = null,
			comment = null,
			usri1_script_path = null
		}; // Create an new instance of the USER_INFO_1 struct
		var result = WinAPI.NETAPI32.NetUserAdd(this.NTCompatibleHostName, 1, ref newUser, 0);
		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to create local user '{0}' on host '{1}'",
				name,
				Host
			);
		}

		var user = new NTLocalUser {
			Host = Host,
			Name = name
		};
		user.Refresh();
		return user;

	}


	public NTShare[] GetShares(out Exception[] errors) {
		errors = new Exception[0];
		var resumeHandle = 0;
		var bufPtr = IntPtr.Zero;
		var localShares = new List<NTShare>();
		var errorList = new List<Exception>();
		try {
			int entriesRead;
			int totalEntries;
			var result = WinAPI.NETAPI32.NetShareEnum(
				this.NTCompatibleHostName,
				0,
				ref bufPtr,
				WinAPI.NETAPI32.MAX_PREFERRED_LENGTH,
				out entriesRead,
				out totalEntries,
				ref resumeHandle
			);

			if (result != 0) {
				throw new NetApiException(
					result,
					"Failed to enumerate shares on host '{0}'",
					Host
				);
			}
			var iter = bufPtr;
			var structSize = Marshal.SizeOf(typeof(WinAPI.NETAPI32.SHARE_INFO_0));
			var startAddr = bufPtr.ToInt64();
			var endAddr = startAddr + entriesRead * structSize;
			for (var offset = startAddr; offset < endAddr; offset += structSize) {
				try {
					var shareInfo =
						(WinAPI.NETAPI32.SHARE_INFO_0)Marshal.PtrToStructure(
							new IntPtr(offset),
							typeof(WinAPI.NETAPI32.SHARE_INFO_0)
						);
					var share = new NTShare {
						Host = Host,
						Name = shareInfo.shi0_netname
					};
					share.Refresh();
					localShares.Add(share);
				} catch (Exception error) {
					errorList.Add(error);
				}
			}
		} finally {
			if (bufPtr != IntPtr.Zero) {
				WinAPI.NETAPI32.NetApiBufferFree(bufPtr);
			}
		}
		errors = errorList.ToArray();
		return localShares.ToArray();
	}

	public NTShare[] GetShares() {
		Exception[] errors;
		return GetShares(out errors);
	}


	public bool TryGetShare(string name, out NTShare share) {
		share = new NTShare();
		if (NTShare.TryLoadLocalShare(NTCompatibleHostName, name, share) == 0) {
			return true;
		} else {
			share = null;
			return false;
		}

	}

	public bool CheckShare(string name, out WinAPI.NETAPI32.SHARE_TYPE shareType) {
		return WinAPI.NETAPI32.NetShareCheck(this.NTCompatibleHostName, name, out shareType) == WinAPI.NETAPI32.NET_API_STATUS.NERR_Success;
	}

	public bool ContainsShare(string name) {
		NTShare share = null;
		return TryGetShare(name, out share);
	}

	public NTShare CreateShare(string name, string description, WinAPI.NETAPI32.SHARE_TYPE type, string path) {
		var newShare = new WinAPI.NETAPI32.SHARE_INFO_502 {
			shi502_netname = name,
			shi502_remark = description,
			shi502_type = (uint)type,
			shi502_path = path
		};
		uint paramErr;
		var result = WinAPI.NETAPI32.NetShareAdd(this.Host, 502, ref newShare, out paramErr);

		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to create local share '{0}' on host '{1}'",
				name,
				Host
			);
		}

		var share = new NTShare {
			Host = Host,
			Name = name
		};
		share.Refresh();
		return share;

	}

	public string ResolveUNCPathToLocalPath(string uncPath) {
		Debug.Assert(uncPath != null);
		var uri = new Uri(uncPath);
		if (!uri.IsUnc) {
			throw new SoftwareException("Unable to resolve the local path of the UNC path '{0}' as the supplied UNC path was invalid", uncPath);
		}

		var sharePath = Uri.UnescapeDataString(uri.AbsolutePath).Split('/');
		if (sharePath.Length <= 1) {
			throw new SoftwareException("Unable to resolve the local path of the UNC path '{0}' as the supplied UNC path was invalid", uncPath);
		}
		var shareName = sharePath[1];
		NTShare share;
		if (!TryGetShare(shareName, out share)) {
			throw new SoftwareException("Unable to resolve local path of UNC path '{0}' as the share '{1}' could not be retrieved on host '{2}", uncPath, shareName, Name);
		}
		var localPath = new StringBuilder();
		localPath.Append(share.ServerPath);
		for (var i = 2; i < sharePath.Length; i++) {
			if (!localPath.ToString().EndsWith(Path.DirectorySeparatorChar.ToString())) {
				localPath.Append(Path.DirectorySeparatorChar);
			}
			localPath.Append(sharePath[i]);
		}
		return localPath.ToString();
	}

	public static string ResolveUNCPathToHostLocalPath(string uncPath) {
		Debug.Assert(uncPath != null);
		var uri = new Uri(uncPath);
		if (!uri.IsUnc) {
			throw new SoftwareException("Unable to resolve the local path of the UNC path '{0}' as the supplied UNC path was invalid", uncPath);
		}
		return new NTHost(uri.Host).ResolveUNCPathToLocalPath(uncPath);
	}


}
