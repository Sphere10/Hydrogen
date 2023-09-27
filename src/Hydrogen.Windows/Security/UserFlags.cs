// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.Security;

[Flags]
public enum UserFlags {
	/// <summary>
	/// The logon script executed. This value must be set.
	/// </summary>
	Script = 0x0001,

	/// <summary>
	/// The user's account is disabled.
	/// </summary>
	AccountDisable = 0x0002,

	/// <summary>
	/// The home directory is required. This value is ignored.
	/// </summary>
	HomeDirRequired = 0x0008,

	/// <summary>
	/// The account is currently locked out. You can call the NetUserSetInfo function to clear this value and unlock a previously locked account. You cannot use this value to lock a previously unlocked account.
	/// </summary>
	Lockout = 0x0010,

	/// <summary>
	/// No password is required.
	/// </summary>
	PasswordNotRequired = 0x0020,

	/// <summary>
	/// The user cannot change the password.
	/// </summary>
	PasswordCantChange = 0x0040,

	/// <summary>
	/// The user's password is stored under reversible encryption in the Active Directory.
	/// </summary>
	/// <remarks>Windows NT: not supported</remarks>
	EncryptedTextPasswordAllowed = 0x0080,

	/// <summary>
	/// The password should never expire on the account
	/// </summary>
	DontExpirePassword = 0x10000,

	/// <summary>
	/// No documentation available for this flag.
	/// </summary>
	/// <remarks>Undocumented Win32 Flag.</remarks>
	MNSLogonAccount = 0x20000,

	/// <summary>
	/// Requires the user to log on to the user account with a smart card.
	/// </summary>
	/// <remarks>Windows NT: not supported</remarks>
	SmartCardRequired = 0x40000,

	/// <summary>
	/// The account is enabled for delegation. This is a security-sensitive setting; accounts with this option enabled should be tightly controlled. This setting allows a service running under the account to assume a client's identity and authenticate as that user to other remote servers on the network.
	/// </summary>
	/// <remarks>Windows NT: not supported</remarks>
	TrustedForDelegation = 0x80000,

	/// <summary>
	/// Marks the account as "sensitive"; other users cannot act as delegates of this user account.
	/// </summary>
	/// <remarks>Windows NT: not supported</remarks>
	NotDelegated = 0x100000,

	/// <summary>
	/// Restrict this principal to use only Data Encryption Standard (DES) encryption types for keys.
	/// </summary>
	/// <remarks>Windows NT: not supported</remarks>
	UseDESKeyOnly = 0x200000,

	/// <summary>
	/// This account does not require Kerberos preauthentication for logon.
	/// </summary>
	/// <remarks>Windows NT: not supported</remarks>
	DontRequirePreAuthentication = 0x400000,

	/// <summary>
	/// The user's password has expired.
	/// </summary>
	/// <remarks>Windows 2000/NT: not supported</remarks>
	PasswordExpired = 0x800000,

	/// <summary>
	/// The account is trusted to authenticate a user outside of the Kerberos security package and delegate that user through constrained delegation. This is a security-sensitive setting; accounts with this option enabled should be tightly controlled. This setting allows a service running under the account to assert a client's identity and authenticate as that user to specifically configured services on the network.
	/// </summary>
	/// <remarks>Windows XP/2000/NT: not supported</remarks>
	TrustedToAuthenticateForDelegation = 0x1000000,

	/// <summary>
	/// This is an account for users whose primary account is in another domain. This account provides user access to this domain, but not to any domain that trusts this domain. The User Manager refers to this account type as a local user account.
	/// </summary>
	/// <remarks>The following flags are mutually exclusive: TempDuplicateAccount, NormalAccount, InterDomainTrustAccount, WorkstationTrustAccount, ServerTrustAccount</remarks>
	TempDuplicateAccount = 0x0100,

	/// <summary>
	/// This is a default account type that represents a typical user.
	/// </summary>
	/// <remarks>The following flags are mutually exclusive: TempDuplicateAccount, NormalAccount, InterDomainTrustAccount, WorkstationTrustAccount, ServerTrustAccount. You cannot change the account type after creation.</remarks>
	NormalAccount = 0x0200,

	/// <summary>
	/// This is a permit to trust account for a domain that trusts other domains.
	/// </summary>
	/// <remarks>The following flags are mutually exclusive: TempDuplicateAccount, NormalAccount, InterDomainTrustAccount, WorkstationTrustAccount, ServerTrustAccount. You cannot change the account type after creation.</remarks>
	InterDomainTrustAccount = 0x0800,

	/// <summary>
	/// This is a computer account for a computer that is a member of this domain.
	/// <remarks>The following flags are mutually exclusive: TempDuplicateAccount, NormalAccount, InterDomainTrustAccount, WorkstationTrustAccount, ServerTrustAccount. You cannot change the account type after creation.</remarks>
	/// </summary>
	WorkstationTrustAccount = 0x1000,

	/// <summary>
	/// This is a computer account for a backup domain controller that is a member of this domain.
	/// </summary>
	/// <remarks>The following flags are mutually exclusive: TempDuplicateAccount, NormalAccount, InterDomainTrustAccount, WorkstationTrustAccount, ServerTrustAccount. You cannot change the account type after creation.</remarks>
	ServerTrustAccount = 0x2000

}
