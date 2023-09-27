// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Linq;


namespace Hydrogen.Windows.Security;

/// <summary>
/// Encapsulates a Local Group from some host machine. A local group can have local user, domain user, and
/// domain groups as members
/// </summary>
public class NTLocalGroup : NTLocalObject {

	/// <summary>
	/// Gets all the members of the group which are other local users or local groups.
	/// </summary>
	/// <returns></returns>
	public NTLocalObject[] GetLocalMembers() {
		return (
			from obj in GetMembers()
			where obj is NTLocalObject
			select (NTLocalObject)obj
		).ToArray();

	}

	/// <summary>
	/// Gets all the members which are active directory objects.
	/// </summary>
	/// <returns></returns>
	public NTRemoteObject[] GetRemoteMembers() {
		return (
			from obj in GetMembers()
			where obj is NTRemoteObject
			select (NTRemoteObject)obj
		).ToArray();

	}

	/// <summary>
	/// Gets all the members which have SIDs but cannot be resolved.
	/// </summary>
	/// <returns></returns>
	public NTDanglingObject[] GetDanglingMembers() {
		return (
			from obj in GetMembers()
			where obj is NTDanglingObject
			select (NTDanglingObject)obj
		).ToArray();
	}

	/// <summary>
	/// Gets all the members belonging to this group.
	/// </summary>
	/// <returns>List of members.</returns>
	public NTObject[] GetMembers() {
		var members = new List<NTObject>();
		var memberInfoPtr = IntPtr.Zero;
		try {
			var resumeHandle = 0;
			const uint prefmaxlen1 = 0xffffffff;
			uint entriesread1, totalentries1;
			var result =
				WinAPI.NETAPI32.NetLocalGroupGetMembers(this.NTCompatibleHostName, this.Name, 2, out memberInfoPtr, prefmaxlen1, out entriesread1, out totalentries1, out resumeHandle);
			if (result != 0) {
				throw new NetApiException(
					result,
					"Unable to get members of group '{0}' on host '{1}'",
					Name,
					Host
				);
			}
			var structSize = Marshal.SizeOf(typeof(WinAPI.NETAPI32.LOCALGROUP_MEMBERS_INFO_2));
			var startAddr = memberInfoPtr.ToInt64();
			var endAddr = startAddr + (int)totalentries1 * structSize;
			for (var offset = startAddr; offset < endAddr; offset += structSize) {
				var memberInfo =
					(WinAPI.NETAPI32.LOCALGROUP_MEMBERS_INFO_2)Marshal.PtrToStructure(
						new IntPtr(offset),
						typeof(WinAPI.NETAPI32.LOCALGROUP_MEMBERS_INFO_2)
					);

				var memberDomainName = Marshal.PtrToStringAuto(memberInfo.lgrmi2_domainandname);
				var memberDomain = SecurityTool.GetDomainFromDomainUserString(memberDomainName);
				var memberName = SecurityTool.GetUserFromDomainUserString(memberDomainName);
				var memberSid = new SecurityIdentifier(memberInfo.lgrmi2_sid);
				var memberSidUsage = (WinAPI.ADVAPI32.SidNameUse)memberInfo.lgrmi2_sidusage;

				if (memberSidUsage == WinAPI.ADVAPI32.SidNameUse.DeletedAccount || memberSidUsage == WinAPI.ADVAPI32.SidNameUse.Unknown) {

					#region It's Dangling object

					members.Add(
						new NTDanglingObject(
							Host,
							memberSid,
							memberSidUsage
						)
					);

					#endregion

				} else if (memberDomain.ToUpper() != this.Host.ToUpper()) {

					#region It's a remote object

					members.Add(
						new NTRemoteObject(
							Host,
							memberDomain,
							memberName,
							memberSid,
							memberSidUsage
						)
					);

					#endregion

				} else {

					#region It's a local object

					switch (memberSidUsage) {
						case WinAPI.ADVAPI32.SidNameUse.User:
							var user = new NTLocalUser {
								Host = Host,
								Name = memberName,
								SID = memberSid
							};
							user.Refresh();
							members.Add(user);
							break;
						case WinAPI.ADVAPI32.SidNameUse.Group:
						case WinAPI.ADVAPI32.SidNameUse.WellKnownGroup:
							var group = new NTLocalGroup();
							group.Host = Host;
							group.Name = memberName;
							group.Refresh();
							members.Add(group);
							break;
						default:
							members.Add(
								new NTDanglingObject(
									Host,
									memberSid,
									memberSidUsage
								)
							);
							break;
					}

					#endregion

				}
			}
		} finally {
			if (memberInfoPtr != IntPtr.Zero) {
				WinAPI.NETAPI32.NetApiBufferFree(memberInfoPtr);
			}
		}
		return members.ToArray();
	}

	/// <summary>
	/// Adds a user to the group. The user could be a local or domain user.
	/// </summary>
	/// <param name="domainAndName">The username referenced by 'domain\name'.</param>
	/// <remarks>The domain could be the name of an accessible domain or hostmachine.</remarks>
	public void AddMember(string domainAndName) {
		var newGroupMember = new WinAPI.NETAPI32.LOCALGROUP_MEMBERS_INFO_3 {
			lgrmi3_domainandname = domainAndName
		};
		var result = WinAPI.NETAPI32.NetLocalGroupAddMembers(this.NTCompatibleHostName, this.Name, 3, ref newGroupMember, 1);

		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to add member '{0}' to group '{1}' on host '{2}'",
				domainAndName,
				Name,
				Host
			);
		}
	}

	/// <summary>
	/// Adds a local user to the group. The user is local to the host this group is defined in (HOST).
	/// </summary>
	/// <param name="username">The username referenced by 'domain\name'.</param>
	/// <remarks>The domain could be the name of an accessible domain or hostmachine.</remarks>
	public void AddLocalMember(string username) {
		AddMember(string.Format("{0}\\{1}", Host, username));
	}

	public void AddLocalMember(NTLocalUser localUser) {
		if (localUser.Host.ToUpper() != Host.ToUpper()) {
			throw new WindowsException(
				"Could not add user '{0}\\{1}' to group '{1}\\{2}' as the two objects differed by owner host",
				localUser.Host,
				localUser.Name,
				Host,
				Name
			);
		}
		AddLocalMember(localUser.Name);

	}

	/// <summary>
	/// Deletes a member from the security group.
	/// </summary>
	/// <param name="obj">An object returned by one of the get methods</param>
	public void DeleteMember(NTObject obj) {
		// delete by SID if available
		if (obj.SID != null) {
			DeleteMember(obj.SID);
		} else {
			var toDelete = obj.Name;
			if (obj is NTRemoteObject) {
				toDelete = ((NTRemoteObject)obj).Domain + "\\";
			}
			DeleteMember(toDelete);
		}
	}

	public void DeleteMember(SecurityIdentifier sid) {
		var groupMember = new WinAPI.NETAPI32.LOCALGROUP_MEMBERS_INFO_0();
		var sidPtr = IntPtr.Zero;
		;
		try {
			if (WinAPI.ADVAPI32.ConvertStringSidToSid(sid.ToString(), out sidPtr)) {
				groupMember.lgrmi0_sid = sidPtr;
				var result = WinAPI.NETAPI32.NetLocalGroupDelMembers(this.NTCompatibleHostName, this.Name, 0, ref groupMember, 1);
				if (result != 0) {
					throw new NetApiException(
						result,
						"Unable to delete member with SID '{0}' from group '{1}' on host '{2}'",
						sid,
						Name,
						Host
					);
				}
			} else {
				throw new WindowsException(
					"Unable to delete member with SID '{0}' from group '{1}' on host '{2}'. Could not convert SID to native SID structure.",
					sid,
					Name,
					Host
				);
			}
		} finally {
			if (sidPtr != IntPtr.Zero) {
				WinAPI.NETAPI32.NetApiBufferFree(sidPtr);
			}
		}
	}

	public void DeleteLocalMember(string name) {
		DeleteMember(string.Format("{0}\\{1}", Host, name));
	}

	public void DeleteMember(string domainAndName) {
		var groupMember = new WinAPI.NETAPI32.LOCALGROUP_MEMBERS_INFO_3 {
			lgrmi3_domainandname = domainAndName
		};

		var result = WinAPI.NETAPI32.NetLocalGroupDelMembers(
			this.NTCompatibleHostName,
			this.Name,
			3,
			ref groupMember,
			1
		);

		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to delete member '{0}' from group '{1}' on host '{2}'",
				groupMember.lgrmi3_domainandname,
				Name,
				Host
			);
		}
	}

	public override void Update() {
		var groupInfo = new WinAPI.NETAPI32.LOCALGROUP_INFO_1 {
			lgrpi1_name = this.Name,
			lgrpi1_comment = this.Description
		};
		const int parmError = 0;

		var result = WinAPI.NETAPI32.NetLocalGroupSetInfo(this.NTCompatibleHostName, this.Name, 1, ref groupInfo, parmError);

		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to update group '{0}' on host '{1}'",
				Name,
				Host
			);
		}

	}

	public override void Refresh() {
		var result = TryLoadLocalGroup(NTCompatibleHostName, Name, this);

		#region Validation

		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to read group '{0}' on host '{1}'",
				Name,
				Host
			);
		}

		#endregion

	}

	public override void Delete() {
		var result = WinAPI.NETAPI32.NetLocalGroupDel(this.NTCompatibleHostName, this.Name);

		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to delete group '{0}' on host '{1}'",
				Name,
				Host
			);
		}
	}

	internal static WinAPI.NETAPI32.NET_API_STATUS TryLoadLocalGroup(string host, string name, NTLocalGroup group) {
		WinAPI.NETAPI32.NET_API_STATUS result;
		var bufPtr = IntPtr.Zero;
		try {
			result = WinAPI.NETAPI32.NetLocalGroupGetInfo(host, name, 1, out bufPtr);

			if (result != 0) {
				return result;
			}

			var groupInfo = (WinAPI.NETAPI32.LOCALGROUP_INFO_1)Marshal.PtrToStructure(
				bufPtr,
				typeof(WinAPI.NETAPI32.LOCALGROUP_INFO_1)
			);
			group.Host = host;
			group.Name = groupInfo.lgrpi1_name;
			group.Description = groupInfo.lgrpi1_comment;

			#region Get the group SID

			var resolver = new SidNameResolver(group.NTCompatibleHostName);
			WinAPI.ADVAPI32.SidNameUse nameUse;
			SecurityIdentifier sid;
			string resolvedDomain;
			string resolveError;
			if (resolver.TryReverseResolve(group.Name, out sid, out resolvedDomain, out nameUse, out resolveError)) {
				if (nameUse != WinAPI.ADVAPI32.SidNameUse.Group && nameUse != WinAPI.ADVAPI32.SidNameUse.Alias) {
					throw new WindowsException(
						"Group '{0}' on host '{1}' had a non-group and/or non-alias SID name use.",
						group.Name,
						group.Host
					);
				}
				group.SID = sid;
				group.SidNameUsage = nameUse;
			} else {
				throw new WindowsException(
					"Unable to resolve SID for group '{0}' on host '{1}'. {2}",
					group.Name,
					group.Host,
					resolveError
				);
			}

			#endregion

		} finally {
			if (bufPtr != IntPtr.Zero) {
				WinAPI.NETAPI32.NetApiBufferFree(bufPtr);
			}
		}
		return result;
	}

}
