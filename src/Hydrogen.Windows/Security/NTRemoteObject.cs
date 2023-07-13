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
/// Encapsulates a reference to an active directory object. The Host property refers
/// to the machine making the reference.
/// </summary>
public class NTRemoteObject : NTObject {
	string _domain;

	/// <summary>
	/// Contructor.
	/// </summary>
	/// <param name="host">The name of the host this object was encountered in</param>
	/// <param name="domain">The name of the domain this object is defined in (should be different than host)</param>
	/// <param name="name">The name of this object</param>
	/// <param name="sid">The security identifier of ths object</param>
	/// <param name="sidNameUse">What the object actually is as it is defined in the domain</param>
	public NTRemoteObject(string host, string domain, string name, SecurityIdentifier sid, WinAPI.ADVAPI32.SidNameUse sidNameUse)
		: base(host, name, sid, sidNameUse) {
		Domain = domain;
	}


	/// <summary>
	/// The domain this remote object is defined in.
	/// </summary>
	public string Domain {
		get { return _domain; }
		set { _domain = value; }
	}

	public override string ToString() {
		return string.Format("{0}\\{1}", Domain, Name);
	}


}
