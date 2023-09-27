// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Security.Principal;

namespace Hydrogen.Windows.Security;

/// <summary>
/// A dangling user or group which was referenced. Either the object had a SID and no name or a
/// name and no SID.
/// </summary>
public class NTDanglingObject : NTObject {
	/// <summary>
	/// Contructor.
	/// </summary>
	/// <param name="host">The name of the host this object was encountered in</param>
	/// <param name="sid">The name the object had.</param>
	public NTDanglingObject(string host, string name)
		: base(host, name, null, WinAPI.ADVAPI32.SidNameUse.Invalid) {
		NameUse = WinAPI.ADVAPI32.SidNameUse.Invalid;
	}

	/// <summary>
	/// Contructor.
	/// </summary>
	/// <param name="host">The name of the host this object was encountered in</param>
	/// <param name="sid">The security identifier of ths object</param>
	/// <param name="sidNameUse">What the object actually is as it is defined in the domain</param>
	public NTDanglingObject(string host, SecurityIdentifier sid, WinAPI.ADVAPI32.SidNameUse sidNameUse)
		: base(host, string.Empty, sid, WinAPI.ADVAPI32.SidNameUse.Invalid) {
		NameUse = sidNameUse;
	}

	public WinAPI.ADVAPI32.SidNameUse NameUse { get; set; }

	public override string ToString() {
		var name = "Invalid Object";
		if (string.IsNullOrEmpty(Name)) {
			if (SID != null) {
				name = SID.ToString();
			}
		} else {
			name = base.ToString();
		}
		return name;
	}


}
