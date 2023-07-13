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

	public static class NETAPI32 {
		// ReSharper disable InconsistentNaming

		#region Constants

		public const uint MAX_PREFERRED_LENGTH = unchecked((uint)-1);
		public const int MAXIMUM_ALLOWED = 0x02000000;
		public const int TOKEN_ASSIGN_PRIMARY = 0x1;
		public const int TOKEN_DUPLICATE = 0x2;
		public const int TOKEN_IMPERSONATE = 0x4;
		public const int TOKEN_QUERY = 0x8;
		public const int TOKEN_QUERY_SOURCE = 0x10;
		public const int TOKEN_ADJUST_PRIVILEGES = 0x20;
		public const int TOKEN_ADJUST_GROUPS = 0x40;
		public const int TOKEN_ADJUST_DEFAULT = 0x80;
		public const int TOKEN_ALL_ACCESS = TOKEN_ASSIGN_PRIMARY + TOKEN_DUPLICATE + TOKEN_IMPERSONATE + TOKEN_QUERY + TOKEN_QUERY_SOURCE + TOKEN_ADJUST_PRIVILEGES + TOKEN_ADJUST_GROUPS + TOKEN_ADJUST_DEFAULT;
		public const int ANYSIZE_ARRAY = 1;
		public const string SE_RESTORE_NAME = "SeRestorePrivilege";
		public const int SE_PRIVILEGE_ENABLED = 0x2;

		#endregion

		#region Structs

		[StructLayout(LayoutKind.Sequential)]
		public struct SHARE_INFO_0 {
			[MarshalAs(UnmanagedType.LPWStr)] public string shi0_netname;
		};


		[StructLayout(LayoutKind.Sequential)]
		public struct SHARE_INFO_502 {
			[MarshalAs(UnmanagedType.LPWStr)] public string shi502_netname;
			public uint shi502_type;
			[MarshalAs(UnmanagedType.LPWStr)] public string shi502_remark;
			public uint shi502_permissions;
			public uint shi502_max_uses;
			public uint shi502_current_uses;
			[MarshalAs(UnmanagedType.LPWStr)] public string shi502_path;
			[MarshalAs(UnmanagedType.LPWStr)] public string shi502_passwd;
			public uint shi502_reserved;
			public IntPtr shi502_security_descriptor; // SECURITY_DESCRIPTOR
		};


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct LOCALGROUP_MEMBERS_INFO_1 {
			public IntPtr lgrmi1_sid;
			public short lgrmi1_sidusage;
			public IntPtr lgrmi1_name;

		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct LOCALGROUP_MEMBERS_INFO_2 {
			public IntPtr lgrmi2_sid;
			public short lgrmi2_sidusage;
			public IntPtr lgrmi2_domainandname;

		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct LOCALGROUP_INFO_0 {
			public string name;
		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct USER_INFO_0 {
			public string Username;
		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct USER_INFO_1 {
			public string usri1_name;
			public string usri1_password;
			public uint usri1_password_age;
			public uint usri1_priv;
			public string usri1_home_dir;
			public string comment;
			public uint usri1_flags;
			public string usri1_script_path;
		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct LOCALGROUP_INFO_1 {
			public string lgrpi1_name;
			public string lgrpi1_comment;
		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct LOCALGROUP_USERS_INFO_0 {
			public string groupname;
		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct GROUP_USERS_INFO_0 {
			public string grui0_name;
		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct USER_INFO_2 {
			public string usri2_name;
			public string usri2_password;
			public uint usri2_password_age;
			public uint usri2_priv;
			public string usri2_home_dir;
			public string usri2_comment;
			public uint usri2_flags;
			public string usri2_script_path;
			public uint usri2_auth_flags;
			public string usri2_full_name;
			public string usri2_usr_comment;
			public string usri2_parms;
			public string usri2_workstations;
			public uint usri2_last_logon;
			public uint usri2_last_logoff;
			public uint usri2_acct_expires;
			public uint usri2_max_storage;
			public uint usri2_units_per_week;
			public IntPtr usri2_logon_hours;
			public uint usri2_bad_pw_count;
			public uint usri2_num_logons;
			public string usri2_logon_server;
			public uint usri2_country_code;
			public uint usri2_code_page;
		}


		// FROM MICROSOFT CODE
		//[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		//public struct USER_INFO_2 {
		//    public string usri2_name;
		//    public string usri2_password;
		//    public uint usri2_password_age;
		//    public uint usri2_priv;
		//    public string usri2_home_dir;
		//    public string usri2_comment;
		//    public uint usri2_flags;
		//    public string usri2_script_path;
		//    public uint usri2_auth_flags;
		//    public string usri2_full_name;
		//    public string usri2_usr_comment;
		//    public string usri2_parms;
		//    public string usri2_workstations;
		//    public uint usri2_last_logon;
		//    public uint usri2_last_logoff;
		//    public uint usri2_acct_expires;
		//    public uint usri2_max_storage;
		//    public uint usri2_units_per_week;
		//    public IntPtr usri2_logon_hours;
		//    public uint usri2_bad_pw_count;
		//    public uint usri2_num_logons;
		//    public string usri2_logon_server;
		//    public uint usri2_country_code;
		//    public uint usri2_code_page;
		//};


		[StructLayout(LayoutKind.Sequential)]
		public struct ACL {
			public byte AclRevision;
			public byte Sbz1;
			public Int16 AclSize;
			public Int16 AceCount;
			public Int16 Sbz2;
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct USER_INFO_3 {
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_name;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_password;
			public uint usri3_password_age;
			public uint usri3_priv;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_home_dir;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_comment;
			public uint usri3_flags;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_script_path;
			public uint usri3_auth_flags;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_full_name;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_usr_comment;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_parms;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_workstations;
			public uint usri3_last_logon;
			public uint usri3_last_logoff;
			public uint usri3_acct_expires;
			public uint usri3_max_storage;
			public uint usri3_units_per_week;
			public IntPtr usri3_logon_hours;
			public uint usri3_bad_pw_count;
			public uint usri3_num_logons;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_logon_server;
			public uint usri3_country_code;
			public uint usri3_code_page;
			public uint usri3_user_id;
			public uint usri3_primary_group_id;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_profile;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri3_home_dir_drive;
			public uint usri3_password_expired;
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct USER_INFO_4 {
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_name;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_password;
			public uint usri4_password_age;
			public uint usri4_priv;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_home_dir;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_comment;
			public uint usri4_flags;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_script_path;
			public uint usri4_auth_flags;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_full_name;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_usr_comment;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_parms;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_workstations;
			public uint usri4_last_logon;
			public uint usri4_last_logoff;
			public uint usri4_acct_expires;
			public uint usri4_max_storage;
			public uint usri4_units_per_week;
			public IntPtr usri4_logon_hours;
			public uint usri4_bad_pw_count;
			public uint usri4_num_logons;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_logon_server;
			public uint usri4_country_code;
			public uint usri4_code_page;
			public IntPtr usri4_user_sid;
			public uint usri4_primary_group_id;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_profile;
			[MarshalAs(UnmanagedType.LPWStr)] public string usri4_home_dir_drive;
			public uint usri4_password_expired;
		}


		//[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		//public struct LOCALGROUP_MEMBERS_INFO_1
		//{
		//    public IntPtr lgrmi1_sid;
		//    public IntPtr lgrmi1_sidusage;
		//    public IntPtr lgrmi1_name;

		//}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct LOCALGROUP_MEMBERS_INFO_0 {
			public IntPtr lgrmi0_sid;
		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct LOCALGROUP_MEMBERS_INFO_3 {
			public string lgrmi3_domainandname;
		}


		//[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
		//public struct LOCALGROUP_USERS_INFO_0
		//{
		//    [MarshalAs(UnmanagedType.LPWStr)]public string name;
		//}


		[StructLayout(LayoutKind.Sequential)]
		public struct LOCALGROUP_USERS_INFO_1 {
			[MarshalAs(UnmanagedType.LPWStr)] public string name;
			[MarshalAs(UnmanagedType.LPWStr)] public string comment;
		}

		#endregion

		#region Enums & Flags

		/// <summary>
		/// Lmcons.h
		/// #define NET_API_STATUS DWORD
		/// </summary>
		public enum NET_API_STATUS : uint {
			NERR_Success = 0,

			/// <summary>
			/// This computer name is invalid.
			/// </summary>
			NERR_InvalidComputer = 2351,

			/// <summary>
			/// This operation is only allowed on the primary domain controller of the domain.
			/// </summary>
			NERR_NotPrimary = 2226,

			/// <summary>
			/// This operation is not allowed on this special group.
			/// </summary>
			NERR_SpeGroupOp = 2234,

			/// <summary>
			/// This operation is not allowed on the last administrative account.
			/// </summary>
			NERR_LastAdmin = 2452,

			/// <summary>
			/// The password parameter is invalid.
			/// </summary>
			NERR_BadPassword = 2203,

			/// <summary>
			/// The password does not meet the password policy requirements. 
			/// Check the minimum password length, password complexity and password history requirements.
			/// </summary>
			NERR_PasswordTooShort = 2245,

			/// <summary>
			/// The user name could not be found.
			/// </summary>
			NERR_UserNotFound = 2221,
			ERROR_ACCESS_DENIED = 5,
			ERROR_NOT_ENOUGH_MEMORY = 8,
			ERROR_INVALID_PARAMETER = 87,
			ERROR_INVALID_NAME = 123,
			ERROR_INVALID_LEVEL = 124,
			ERROR_MORE_DATA = 234,
			ERROR_SESSION_CREDENTIAL_CONFLICT = 1219,

			/// <summary>
			/// The RPC server is not available. This error is returned if a remote computer was specified in
			/// the lpServer parameter and the RPC server is not available.
			/// </summary>
			RPC_S_SERVER_UNAVAILABLE = 2147944122, // 0x800706BA

			/// <summary>
			/// Remote calls are not allowed for this process. This error is returned if a remote computer was 
			/// specified in the lpServer parameter and remote calls are not allowed for this process.
			/// </summary>
			RPC_E_REMOTE_DISABLED = 2147549468 // 0x8001011C
		}


		public enum SHARE_TYPE : uint {
			STYPE_DISK = 0, // Disk Share
			STYPE_PRINTQ = 1, // Print Queue
			STYPE_DEVICE = 2, // Communication Device 
			STYPE_IPC = 3, // IPC (Interprocess communication) Share
			STYPE_HIDDEN_DISK = 0x80000000, // Admin Disk Shares
			STYPE_HIDDEN_PRINT = 0x80000001, // Admin Print Shares
			STYPE_HIDDEN_DEVICE = 0x80000002, // Admin Device Shares
			STYPE_HIDDEN_IPC = 0x80000003, // Admin IPC Shares
			STYPE_TEMPORARY = unchecked((uint)0x40000000),
			STYPE_SPECIAL = unchecked((uint)0x80000000),
		}


		[Flags]
		public enum GlobalSharePermission {
			ACCESS_NONE = 0,
			ACCESS_ALL = (ACCESS_READ | ACCESS_WRITE | ACCESS_CREATE | ACCESS_EXEC | ACCESS_DELETE | ACCESS_ATRIB | ACCESS_PERM | ACCESS_FINDFIRST),
			ACCESS_READ = 0x1,
			ACCESS_WRITE = 0x2,
			ACCESS_CREATE = 0x4,
			ACCESS_EXEC = 0x8,
			ACCESS_DELETE = 0x10,
			ACCESS_ATRIB = 0x20,
			ACCESS_PERM = 0x40,
			ACCESS_FINDFIRST = 0x80,
			ACCESS_GROUP = 0x8000,
			ACCESS_AUDIT = 0x1,
			ACCESS_ATTR_PARMNUM = 2
		}


		[Flags]
		public enum usri4_flags {
			UF_SCRIPT = 1,
			UF_ACCOUNTDISABLE = 2,
			UF_HOMEDIR_REQUIRED = 8,
			UF_LOCKOUT = 16,
			UF_PASSWD_NOTREQD = 32,
			UF_PASSWD_CANT_CHANGE = 64,
			UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 128,
			UF_PASSWD_NEVER_EXPIRES = 65536

		}


		[Flags]
		public enum usri4_auth_flags {
			AF_OP_PRINT = 1,
			AF_OP_COMM = 2,
			AF_OP_SERVER = 4,
			AF_OP_ACCOUNTS = 8
		}


		public enum TOKEN_INFORMATION_CLASS {
			TokenUser = 1,
			TokenGroups,
			TokenPrivileges,
			TokenOwner,
			TokenPrimaryGroup,
			TokenDefaultDacl,
			TokenSource,
			TokenType,
			TokenImpersonationLevel,
			TokenStatistics,
			TokenRestrictedSids,
			TokenSessionId
		}

		#endregion

		#region NETAPI32 Functions

		[DllImport("netapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetShareDel([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string netname, int reserved);

		[DllImport("netapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetShareAdd([MarshalAs(UnmanagedType.LPWStr)] string strServer, Int32 dwLevel, ref SHARE_INFO_502 buf, out uint parm_err);


		[DllImport("netapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetShareSetInfo([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string netname, int level, ref SHARE_INFO_502 buf, out uint parm_err);

		[DllImport("netapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetShareGetInfo([MarshalAs(UnmanagedType.LPWStr)] string serverName, [MarshalAs(UnmanagedType.LPWStr)] string netName, Int32 level, out IntPtr bufPtr);

		[DllImport("netapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetShareCheck([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string device, [MarshalAs(UnmanagedType.U4)] out SHARE_TYPE type);

		[DllImport("netapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetShareEnum([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, ref IntPtr bufPtr, uint prefmaxlen, out int entriesread, out int totalentries, ref int resume_handle);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetLocalGroupAdd([MarshalAs(UnmanagedType.LPWStr)] string servername, uint level, ref LOCALGROUP_INFO_1 localGroupInfo, ref int error);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetGroupEnum([MarshalAs(UnmanagedType.LPWStr)] string servername, uint level, out SHARE_INFO_0 siPtr, int prefmaxlen, out uint entriesread, out uint totalentries, out int resumeHandle);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetGroupAddUser([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string groupname, [MarshalAs(UnmanagedType.LPWStr)] string username);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetLocalGroupAddMembers([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string groupname, int level, ref LOCALGROUP_MEMBERS_INFO_3 buf, int totalentries);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetLocalGroupDelMembers([MarshalAs(UnmanagedType.LPWStr)] string psServer, [MarshalAs(UnmanagedType.LPWStr)] string psLocalGroup, int lLevel, ref LOCALGROUP_MEMBERS_INFO_0 uMember, int lMemberCount);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetLocalGroupDelMembers([MarshalAs(UnmanagedType.LPWStr)] string psServer, [MarshalAs(UnmanagedType.LPWStr)] string psLocalGroup, int lLevel, ref LOCALGROUP_MEMBERS_INFO_3 uMember, int lMemberCount);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetUserAdd([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, ref USER_INFO_1 buf, int parm_err);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetUserDel([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetLocalGroupDel([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string groupname);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetLocalGroupGetInfo([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string groupname, int level, out IntPtr bufptr);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetLocalGroupSetInfo([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string groupname, int level, ref LOCALGROUP_INFO_1 buf, int parm_err);

		// [DllImport("Netapi32.dll")]
		//public extern static int  NetLocalGroupDelUser(
		//     [MarshalAs(UnmanagedType.LPWStr)] string servername, 
		//     [MarshalAs(UnmanagedType.LPWStr)] string groupname,
		//     [MarshalAs(UnmanagedType.LPWStr)] string username);

		//[DllImport("Netapi32.dll")]
		//public extern static int NetLocalGroupAddUser(
		//     [MarshalAs(UnmanagedType.LPWStr)] string servername,
		//     [MarshalAs(UnmanagedType.LPWStr)] string groupname,
		//     [MarshalAs(UnmanagedType.LPWStr)] string username);


		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetUserGetInfo([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username, int level, out IntPtr bufptr);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetUserSetInfo([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username, int level, ref USER_INFO_2 buf, int error);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetUserEnum([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, int filter, out IntPtr bufptr, int prefmaxlen, out int entriesread, out int totalentries, out int resume_handle);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetLocalGroupGetMembers([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string localgroupname, uint level, out IntPtr bufptr, uint prefmaxlen, out uint entriesread,
		                                                            out uint totalentries, out int resume_handle);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetUserGetGroups([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username, int level, out IntPtr bufptr, int prefmaxlen, out uint entriesread,
		                                                     out uint totalentries);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetGetDCName([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string domainname, IntPtr bufptr);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetUserGetLocalGroups([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username, int level, int flags, out IntPtr bufptr, int prefmaxlen, out int entriesread,
		                                                          out int totalentries);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetApiBufferFree(IntPtr bufptr);

		//[DllImport("netapi32.dll", EntryPoint = "NetLocalGroupGetMembers")]
		//public static extern uint NetLocalGroupGetMembers(
		//    IntPtr ServerName,
		//    IntPtr GrouprName,
		//    uint level,
		//    ref IntPtr siPtr,
		//    int prefmaxlen,
		//    ref uint entriesread,
		//    ref uint totalentries,
		//    IntPtr resumeHandle);

		[DllImport("netapi32.dll")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern NET_API_STATUS NetLocalGroupEnum([MarshalAs(UnmanagedType.LPWStr)] string servername, uint level, out IntPtr siPtr, int prefmaxlen, out uint entriesread, out uint totalentries, out int resumeHandle);

		#endregion

		// ReSharper restore InconsistentNaming
	}

}
