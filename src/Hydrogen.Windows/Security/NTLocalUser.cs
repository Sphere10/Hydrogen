// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Runtime.InteropServices;


namespace Hydrogen.Windows.Security;

/// <summary>
/// Encapsulates a Local User from some host machine. A local user can only be a member of local groups.
/// </summary>
public class NTLocalUser : NTLocalObject {
	string _password;
	uint _password_age;
	uint _priv;
	string _home_dir;
	string _comment;
	uint _flags;
	string _script_path;
	uint _auth_flags;
	string _full_name;
	string _parms;
	string usri2_workstations;
	uint _last_logon;
	uint _last_logoff;
	uint _acct_expires;
	string _logon_server;

	internal NTLocalUser() {
		LogonHours = new byte[21];
	}

	/// <summary>
	/// Users password. Can only set the value.
	/// </summary>
	public string Password {
		set { _password = value; }
	}

	/// <summary>
	/// The number of seconds that have elapsed since the Password was last changed.
	/// </summary>
	public TimeSpan PasswordAge {
		get { return TimeSpan.FromSeconds(_password_age); }
	}

	/// <summary>
	/// Specifies level of privilege assigned to the user.  
	/// </summary>
	public UserPrivilege Privilege {
		get { return (UserPrivilege)_priv; }
	}

	/// <summary>
	/// Specifies the path of the home directory for the user. The string can be null.
	/// </summary>
	public string HomeDirectory {
		get { return _home_dir; }
		set { _home_dir = value; }
	}

	/// <summary>
	/// A user comment. The string can be a null string, or it can have any number of characters before the terminating null character.
	/// </summary>
	/// <remarks>User.Description contains the general comment associated with this user.</remarks>
	public string Comment {
		get { return _comment; }
		set { _comment = value; }
	}

	/// <summary>
	/// These flags determine several features and restrictions applied to the user. 
	/// </summary>
	public UserFlags Flags {
		get { return (UserFlags)_flags; }
		set { _flags = (uint)value; }
	}

	/// <summary>
	/// Specifies the path for the user's logon script file. The script file can be a .CMD file, an .EXE file, or a .BAT file. The string can also be null.
	/// </summary>
	public string ScriptPath {
		get { return _script_path; }
		set { _script_path = value; }
	}

	/// <summary>
	/// The user's operator privileges. 
	/// </summary>
	public UserOperatorPriviliges OperatorPrivileges {
		get { return (UserOperatorPriviliges)_auth_flags; }
	}

	/// <summary>
	/// The full name of the user. This string can be a null string, or it can have any number of characters before the terminating null character.
	/// </summary>
	public override string FullName {
		get { return _full_name; }
		set { _full_name = value; }
	}

	/// <summary>
	/// Reserved for use by applications. This string can be a null string, or it can have any number of characters before the terminating null character. Microsoft products use this member to store user configuration information. Do not modify this information.
	/// </summary>
	public string Reserved {
		get { return _parms; }
	}

	/// <summary>
	/// List of workstations the user can log on from. To disable logons from all workstations to this account, set the 'UserFlags.AccountDisable' value in the User.Flags member.
	/// </summary>
	public string[] Workstations {
		get { return usri2_workstations.Split(',').TrimMany().ToArray(); }
		set { usri2_workstations = value.ToDelimittedString(","); }
	}

	/// <summary>
	/// Specifies when the last logon occurred. A value of DateTime.MinValue indicates that the last logon time is unknown.
	/// This member is maintained separately on each backup domain controller (BDC) in the domain. To obtain an accurate value, you must query each BDC in the domain. The last logon occurred at the time indicated by the largest retrieved value.
	/// </summary>
	public DateTime LastLogon {
		get { return (_last_logon == 0) ? DateTime.MinValue : Tools.Time.DateFrom1979Jan1(_last_logon); }
	}

	/// <summary>
	/// Specifies when the last logoff occurred. A value of DateTime.MinValue indicates that the last logoff time is unknown.
	/// This member is maintained separately on each backup domain controller (BDC) in the domain. To obtain an accurate value, you must query each BDC in the domain. The last logoff occurred at the time indicated by the largest retrieved value. This member is currently not used. 
	/// </summary>
	public DateTime LastLogoff {
		get { return (_last_logoff == 0) ? DateTime.MinValue : Tools.Time.DateFrom1979Jan1(_last_logoff); }
	}

	/// <summary>
	/// Specifies when the account expires. This value is stored as the number of seconds elapsed since 00:00:00, January 1, 1970, GMT. 
	/// </summary>
	/// <remarks>A value of DateTime.MaxValue indicates that the account never expires.</remarks>
	public DateTime AccountExpires {
		get {
			return
				(_acct_expires == unchecked((uint)-1)) ? DateTime.MaxValue : Tools.Time.DateFrom1979Jan1(_acct_expires);
		}
		set {
			_acct_expires =
				(value == DateTime.MaxValue) ? unchecked((uint)-1) : value.MilliSecondsSince1979Jan1();
		}
	}


	/// <summary>
	/// Specifies a the maximum amount of disk space the user can use. A value of -1 indicates unlimited streams.
	/// </summary>
	public uint MaxStorage { get; set; }


	/// <summary>
	/// Specifies a DWORD value that indicates the number of equal-length time units into which the week is divided. This value is required to compute the length of the bit string in the User.LogonHours member. 
	/// </summary>
	public uint UnitsPerWeek { get; set; }


	/// <summary>
	/// A 21-byte (168 bits) bit string that specifies the times during which the user can log on. Each bit represents a unique hour in the week, in Greenwich Mean Time (GMT). 
	///
	/// The first bit (bit 0, word 0) is Sunday, 0:00 to 0:59; the second bit (bit 1, word 0) is Sunday, 1:00 to 1:59; and so on. Note that bit 0 in word 0 represents Sunday from 0:00 to 0:59 only if you are in the GMT time zone. In all other cases you must adjust the bits according to your time zone offset (for example, GMT minus 8 hours for Pacific Standard Time).
	/// </summary>
	/// <remarks>A null pointer indicates no time restriction, or no updates to existing time restrictions when updating.</remarks>
	public byte[] LogonHours { get; set; }

	/// <summary>
	/// The number of times the user tried to log on to the account using an incorrect password. A value of –1 indicates that the value is unknown.
	/// 
	/// This member is replicated from the primary domain controller (PDC); it is also maintained on each backup domain controller (BDC) in the domain. To obtain an accurate value, you must query each BDC in the domain. The number of times the user tried to log on using an incorrect password is the largest value retrieved.
	/// </summary>
	public uint BadPasswordCount { get; set; }

	/// <summary>
	/// The number of times the user logged on successfully to this account. A value of –1 indicates that the value is unknown. 
	/// This member is maintained separately on each backup domain controller (BDC) in the domain. To obtain an accurate value, you must query each BDC in the domain. The number of times the user logged on successfully is the sum of the retrieved values.
	/// </summary>
	public uint NumberOfLogons { get; set; }

	/// <summary>
	/// String that contains the name of the server to which logon requests are sent. Server names should be preceded by two backslashes (\\). To indicate that the logon request can be handled by any logon server, specify an asterisk (\\*) for the server name. A null string indicates that requests should be sent to the domain controller. 
	/// For Windows servers, this value starts with '\\'.
	/// </summary>
	public string LogonServer {
		get { return _logon_server; }
		set { _logon_server = value; }
	}

	/// <summary>
	/// Country/region code for the user's language of choice.
	/// </summary>
	public uint CountryCode { get; set; }

	/// <summary>
	///Code page for the user's language of choice.
	/// </summary>
	public uint CodePage { get; set; }

	/// <summary>
	/// Gets all the security groups this object has membership to.
	/// </summary>
	/// <returns></returns>
	public NTLocalGroup[] GetMembership() {
		var bufPtr = IntPtr.Zero;
		var members = new List<NTLocalGroup>();
		try {
			int entriesRead, totalEntries;
			var result =
				WinAPI.NETAPI32.NetUserGetLocalGroups(
					this.Host,
					this.Name,
					0,
					0,
					out bufPtr,
					8192,
					out entriesRead,
					out totalEntries
				);

			if (result != 0) {
				throw new NetApiException(
					result,
					"Unable to get user '{0}' list of membership on host '{1}'",
					Name,
					Host
				);
			}

			var structSize = Marshal.SizeOf(typeof(WinAPI.NETAPI32.LOCALGROUP_USERS_INFO_0));
			var startAddr = bufPtr.ToInt64();
			var endAddr = startAddr + (int)totalEntries * structSize;
			for (var i = startAddr; i < endAddr; i += structSize) {
				var groupInfo
					= (WinAPI.NETAPI32.LOCALGROUP_USERS_INFO_0)Marshal.PtrToStructure(
						new IntPtr(i),
						typeof(WinAPI.NETAPI32.LOCALGROUP_USERS_INFO_0)
					);

				#region Local object

				NTLocalGroup localGroup = new NTLocalGroup();
				localGroup.Host = Host;
				localGroup.Name = groupInfo.groupname;
				localGroup.Refresh();
				members.Add(localGroup);

				#endregion

			}

		} finally {
			if (bufPtr != IntPtr.Zero) {
				WinAPI.NETAPI32.NetApiBufferFree(bufPtr);
			}
		}
		return members.ToArray();
	}

	/// <summary>
	/// Makes the user a member of the specified local group.
	/// </summary>
	/// <param name="groupName">The local name of the group.</param>
	/// <remarks>The group must be local to the machine this user is defined on (i.e. User.Host)</remarks>
	public void AddMembership(string groupName) {

		var newGroupMember = new WinAPI.NETAPI32.LOCALGROUP_MEMBERS_INFO_3 { lgrmi3_domainandname = this.Name };
		var result = WinAPI.NETAPI32.NetLocalGroupAddMembers(
			this.Host,
			groupName,
			3,
			ref newGroupMember,
			1
		);

		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to add group membership '{0}' to user '{1}' on host '{2}'",
				groupName,
				Name,
				Host
			);
		}
	}

	public void AddMembership(NTLocalGroup localGroup) {
		if (localGroup.Host.ToUpper() != Host.ToUpper()) {
			throw new WindowsException(
				"Could not add group membership '{0}\\{1}' to user '{1}\\{2}' as the two objects differed by owner host",
				localGroup.Host,
				localGroup.Name,
				Host,
				Name
			);
		}
		AddMembership(localGroup.Name);
	}

	public override void Update() {
		var userInfo = new WinAPI.NETAPI32.USER_INFO_2();
		var logonHoursUnmanaged = IntPtr.Zero;

		try {
			if (LogonHours.Length > 0) {
				logonHoursUnmanaged = Marshal.AllocHGlobal(21);
				if (logonHoursUnmanaged == IntPtr.Zero) {
					throw new WindowsException("Unable to allocate 21 bytes of unmanaged memory");
				}
				Marshal.Copy(LogonHours, 0, logonHoursUnmanaged, 21);
			}


			userInfo.usri2_acct_expires = _acct_expires;
			userInfo.usri2_auth_flags = _auth_flags;
			userInfo.usri2_bad_pw_count = BadPasswordCount;
			userInfo.usri2_code_page = CodePage;
			userInfo.usri2_comment = Description;
			userInfo.usri2_country_code = CountryCode;
			userInfo.usri2_flags = _flags;
			userInfo.usri2_full_name = _full_name;
			userInfo.usri2_home_dir = _home_dir;
			userInfo.usri2_usr_comment = _comment;
			userInfo.usri2_last_logoff = _last_logoff;
			userInfo.usri2_last_logon = _last_logon;
			userInfo.usri2_logon_hours = logonHoursUnmanaged;
			userInfo.usri2_logon_server = _logon_server;
			userInfo.usri2_max_storage = MaxStorage;
			userInfo.usri2_name = Name;
			userInfo.usri2_num_logons = NumberOfLogons;
			userInfo.usri2_parms = _parms;
			userInfo.usri2_password = _password;
			userInfo.usri2_password_age = _password_age; // ignored anyway
			userInfo.usri2_priv = _priv;
			userInfo.usri2_script_path = _script_path;
			userInfo.usri2_units_per_week = UnitsPerWeek;
			userInfo.usri2_workstations = usri2_workstations;

			const int parmError = 0;

			var result = WinAPI.NETAPI32.NetUserSetInfo(
				this.Host,
				this.Name,
				2,
				ref userInfo,
				parmError
			);

			if (result != 0) {
				throw new NetApiException(
					result,
					"Unable to update user '{0}' on host '{1}'",
					Name,
					Host
				);
			}
		} finally {
			if (logonHoursUnmanaged != IntPtr.Zero) {
				Marshal.FreeHGlobal(logonHoursUnmanaged);
			}
		}


	}

	public override void Refresh() {

		var result = TryLoadLocalUser(this.NTCompatibleHostName, this.Name, this);

		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to read user '{0}' on host '{1}'",
				Name,
				Host
			);
		}
	}

	public override void Delete() {
		var result = WinAPI.NETAPI32.NetUserDel(this.Host, this.Name);
		if (result != 0) {
			throw new NetApiException(
				result,
				"Unable to delete user '{0}' on host '{2}'",
				Name,
				Host
			);
		}
	}

	internal static WinAPI.NETAPI32.NET_API_STATUS TryLoadLocalUser(string host, string username, NTLocalUser user) {
		WinAPI.NETAPI32.NET_API_STATUS result;
		var bufPtr = IntPtr.Zero;
		try {
			result = WinAPI.NETAPI32.NetUserGetInfo(
				host,
				username,
				2,
				out bufPtr
			);

			if (result != 0) {
				return result;
			}

			var userInfo = (WinAPI.NETAPI32.USER_INFO_2)Marshal.PtrToStructure(bufPtr, typeof(WinAPI.NETAPI32.USER_INFO_2));

			user.Host = host;
			user._acct_expires = userInfo.usri2_acct_expires;
			user._auth_flags = userInfo.usri2_auth_flags;
			user.BadPasswordCount = userInfo.usri2_bad_pw_count;
			user.CodePage = userInfo.usri2_code_page;
			user.Description = userInfo.usri2_comment;
			user.CountryCode = userInfo.usri2_country_code;
			user._flags = userInfo.usri2_flags;
			user._full_name = userInfo.usri2_full_name;
			user._home_dir = userInfo.usri2_home_dir;
			user._comment = userInfo.usri2_usr_comment;
			user._last_logoff = userInfo.usri2_last_logoff;
			user._last_logon = userInfo.usri2_last_logon;
			user._logon_server = userInfo.usri2_logon_server;
			user.MaxStorage = userInfo.usri2_max_storage;
			user.Name = userInfo.usri2_name;
			user.NumberOfLogons = userInfo.usri2_num_logons;
			user._parms = userInfo.usri2_parms;
			user._password = userInfo.usri2_password;
			user._password_age = userInfo.usri2_password_age;
			user._priv = userInfo.usri2_priv;
			user._script_path = userInfo.usri2_script_path;
			user.UnitsPerWeek = userInfo.usri2_units_per_week;
			user.usri2_workstations = userInfo.usri2_workstations;

			if (userInfo.usri2_logon_hours != IntPtr.Zero) {
				Marshal.Copy(userInfo.usri2_logon_hours, user.LogonHours, 0, 21);
				WinAPI.NETAPI32.NetApiBufferFree(userInfo.usri2_logon_hours);
			} else {
				user.LogonHours = new byte[0];
			}

			#region Get the group SID

			var resolver = new SidNameResolver(user.NTCompatibleHostName);
			WinAPI.ADVAPI32.SidNameUse nameUse;
			SecurityIdentifier sid;
			string resolvedDomain;
			string resolveError;
			if (resolver.TryReverseResolve(user.Name, out sid, out resolvedDomain, out nameUse, out resolveError)) {
				if (nameUse != WinAPI.ADVAPI32.SidNameUse.User && nameUse != WinAPI.ADVAPI32.SidNameUse.Alias) {
					throw new WindowsException(
						"User '{0}' on host '{1}' had a non-user and non-alias SID name",
						user.Name,
						user.Host
					);
				}
				user.SID = sid;
				user.SidNameUsage = nameUse;
			} else {
				throw new WindowsException(
					"Unable to resolve SID for user '{0}' on host '{1}. {2}",
					user.Name,
					user.Host,
					resolveError
				);

			}

			#endregion

		} finally {
			WinAPI.NETAPI32.NetApiBufferFree(bufPtr);
		}
		return result;
	}


}
