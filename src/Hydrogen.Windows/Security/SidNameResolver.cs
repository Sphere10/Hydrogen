// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.Runtime.InteropServices;


namespace Hydrogen.Windows.Security;

/// <summary>
/// Resolves SID string into it's symbolic name by querying a list of remote servers.
/// </summary>
public class SidNameResolver {
	readonly List<string> _remoteHosts;
	readonly IDictionary<string, Tuple<string, string, WinAPI.ADVAPI32.SidNameUse>> _resolvedSIDs;
	readonly IDictionary<string, Tuple<SecurityIdentifier, string, WinAPI.ADVAPI32.SidNameUse>> _resolvedNames;
	readonly IDictionary<string, bool> _resolvedWithoutLookup;


	/// <summary>
	/// Only resolves on local machine.
	/// </summary>
	public SidNameResolver()
		: this(true, null) {

	}

	/// <summary>
	/// Only resolves on remote machine.
	/// </summary>
	/// <param name="remoteMachine"></param>
	public SidNameResolver(string remoteMachine)
		: this(false, new string[1] { remoteMachine }) {
	}

	/// <summary>
	/// Can resolve on local and many other remote machines.
	/// </summary>
	/// <param name="includeLocalMachine"></param>
	/// <param name="remoteHosts"></param>
	public SidNameResolver(bool includeLocalMachine, string[] remoteHosts) {
		_remoteHosts = new List<string>();
		if (remoteHosts != null) {
			_remoteHosts.AddRange(remoteHosts);
		}
		if (includeLocalMachine) {
			_remoteHosts.Insert(0, null);
		}
		_resolvedSIDs = new Dictionary<string, Tuple<string, string, WinAPI.ADVAPI32.SidNameUse>>();
		_resolvedNames = new Dictionary<string, Tuple<SecurityIdentifier, string, WinAPI.ADVAPI32.SidNameUse>>();
		_resolvedWithoutLookup = new Dictionary<string, bool>();
	}


	/// <summary>
	/// Attempt to resolve a SID string into a symbolic name.
	/// </summary>
	/// <param name="sid">The string representation of the SID</param>
	/// <param name="remoteHost">The host which the name is defined at.</param>
	/// <param name="resolvedName">The name the SID resolves to on the host.</param>
	/// <param name="sidNameUse">What the host uses the name for.</param>
	/// <returns></returns>
	public bool TryResolve(string sid, out string remoteHost, out string resolvedName, out WinAPI.ADVAPI32.SidNameUse sidNameUse) {
		var retval = false;
		remoteHost = string.Empty;
		resolvedName = string.Empty;
		sidNameUse = WinAPI.ADVAPI32.SidNameUse.Alias;

		if (_resolvedSIDs.ContainsKey(sid)) {
			remoteHost = _resolvedSIDs[sid].Item1;
			resolvedName = _resolvedSIDs[sid].Item2;
			sidNameUse = _resolvedSIDs[sid].Item3;
			retval = true;
		} else {
			foreach (var host in _remoteHosts) {
				if (LookupAccountSid(host, sid, out resolvedName, out remoteHost, out sidNameUse)) {
					_resolvedSIDs[sid] = new Tuple<string, string, WinAPI.ADVAPI32.SidNameUse>(
						remoteHost,
						resolvedName,
						sidNameUse
					);
					remoteHost = _resolvedSIDs[sid].Item1;
					resolvedName = _resolvedSIDs[sid].Item2;
					sidNameUse = _resolvedSIDs[sid].Item3;
					retval = true;
					break;
				}
			}
		}
		return retval;
	}


	//public static bool LookupAccountName(
	//    string strServer,
	//    string strAccountName,
	//    out SecurityIdentifier accountSid,
	//    out string strDomainName,
	//    out SidNameUse sidNameUse) {
	//    string error;
	//    return LookupAccountName(
	//        strServer,
	//        strAccountName,
	//        out accountSid,
	//        out strDomainName,
	//        out sidNameUse,
	//        out error
	//    );
	//}

	public bool TryReverseResolve(
		string accountName,
		out SecurityIdentifier resolvedSid,
		out string resolvedDomain,
		out WinAPI.ADVAPI32.SidNameUse sidNameUse,
		out string error) {

		error = string.Empty;
		bool retval = false;
		sidNameUse = WinAPI.ADVAPI32.SidNameUse.Invalid;
		resolvedSid = null;
		resolvedDomain = null;


		if (_resolvedNames.Keys.Contains(accountName)) {
			resolvedSid = _resolvedNames[accountName].Item1;
			resolvedDomain = _resolvedNames[accountName].Item2;
			sidNameUse = _resolvedNames[accountName].Item3;
			retval = true;
		} else {
			// go through each host and try to lookup the account name
			foreach (string host in _remoteHosts) {
				retval = LookupAccountName(
					host,
					accountName,
					out resolvedSid,
					out resolvedDomain,
					out sidNameUse,
					out error
				);
				if (retval) {
					_resolvedNames[accountName] =
						new Tuple<SecurityIdentifier, string, WinAPI.ADVAPI32.SidNameUse>(
							resolvedSid,
							resolvedDomain,
							sidNameUse
						);
				}
			}
		}
		return retval;
	}


	public static bool LookupAccountName(
		string strServer,
		string strAccountName,
		out SecurityIdentifier accountSid,
		out string strDomainName,
		out WinAPI.ADVAPI32.SidNameUse sidNameUse,
		out string error) {
		error = string.Empty;
		int win32Error;
		var retval = _LookupAccountName(
			strServer,
			strAccountName,
			out accountSid,
			out strDomainName,
			out sidNameUse,
			out win32Error
		);
		if (win32Error != 0) {
			error = WindowsException.ErrorCodeToDescription(win32Error);
		}
		return retval;
	}


	private static bool _LookupAccountName(
		string strServer,
		string strAccountName,
		out SecurityIdentifier accountSid,
		out string strDomainName,
		out WinAPI.ADVAPI32.SidNameUse sidNameUse,
		out int win32Error) {
		win32Error = 0;
		var bRet = false;

		uint lDomainNameSize = 256;
		accountSid = null;
		strDomainName = "";
		sidNameUse = 0;
		uint lSidSize = 0;
		var sid = IntPtr.Zero;

		// First get the required buffer sizes for SID and domain name.
		try {
			bRet = WinAPI.ADVAPI32.LookupAccountName(
				strServer,
				strAccountName,
				sid,
				ref lSidSize,
				null,
				ref lDomainNameSize,
				out sidNameUse
			);


			if (!bRet) {
				var nErr = Marshal.GetLastWin32Error();
				if (122 == nErr) {
					// Buffer too small
					// Allocate the buffers with actual sizes that are required
					// for SID and domain name.
					var strName = new StringBuilder((int)lDomainNameSize);
					sid = Marshal.AllocHGlobal((int)lSidSize);
					bRet = WinAPI.ADVAPI32.LookupAccountName(
						strServer,
						strAccountName,
						sid,
						ref lSidSize,
						strName,
						ref lDomainNameSize,
						out sidNameUse);

					if (bRet) {
						strDomainName = strName.ToString();
						accountSid = new SecurityIdentifier(sid);
					} else {
						win32Error = Marshal.GetLastWin32Error();
					}
				} else {
					win32Error = nErr;
				}
			}
		} finally {
			if (sid != IntPtr.Zero) {
				Marshal.FreeHGlobal(sid);
			}
		}
		return bRet;
	}

	public static bool LookupAccountSid(
		string host,
		string sid,
		out string name,
		out string domain,
		out WinAPI.ADVAPI32.SidNameUse sidNameUse
	) {
		name = null;
		domain = null;
		sidNameUse = WinAPI.ADVAPI32.SidNameUse.Invalid;
		var retval = false;
		var sidPtr = IntPtr.Zero;
		try {
			if (WinAPI.ADVAPI32.ConvertStringSidToSid(sid, out sidPtr)) {
				var nameBuilder = new StringBuilder(1024);
				var domainBuilder = new StringBuilder(1024);
				var nameCapacity = (uint)nameBuilder.Capacity;
				var domainCapacity = (uint)domainBuilder.Capacity;
				retval = WinAPI.ADVAPI32.LookupAccountSid(
					host,
					sidPtr,
					nameBuilder,
					ref nameCapacity,
					domainBuilder,
					ref domainCapacity,
					out sidNameUse
				);
				name = nameBuilder.ToString();
				domain = domainBuilder.ToString();
			}
		} finally {
			if (sidPtr != IntPtr.Zero) {
				WinAPI.NETAPI32.NetApiBufferFree(sidPtr);
			}
		}
		return retval;
	}


	/// <summary>
	/// Whether or not given SID can be resolved to a symbolic name within connected
	/// domains and local machine.
	/// </summary>
	/// <param name="sid"></param>
	/// <returns></returns>
	public bool IsSIDResolvableWithoutLookup(SecurityIdentifier sid) {
		bool retval = false;
		string sidString = sid.ToString();
		try {
			if (_resolvedWithoutLookup.ContainsKey(sidString)) {
				retval = _resolvedWithoutLookup[sidString];
			} else if (sid.Translate(typeof(NTAccount)) is NTAccount) {
				retval = true;
			}
		} catch {
		}
		// could not translate SID to nt account, thus it belongs to a remote host or domain
		_resolvedWithoutLookup[sidString] = retval;
		return retval;
	}


}
