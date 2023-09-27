// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Security.Principal;


namespace Hydrogen.Windows.Security;

/// <summary>
/// Base class for objects which have a SID and name and host machine they are on.
/// </summary>
public abstract class NTObject {
	private string _host;
	private string _name;

	public NTObject()
		: this(string.Empty, string.Empty, null, WinAPI.ADVAPI32.SidNameUse.Unknown) {
	}

	public NTObject(string host, string name, SecurityIdentifier sid, WinAPI.ADVAPI32.SidNameUse sidNameUsage) {
		Host = host;
		Name = name;
		SID = sid;
		SidNameUsage = sidNameUsage;
	}

	public string Host {
		get { return _host; }
		internal set { _host = value.TrimStart('\\'); }
	}

	public string Name {
		get { return _name; }
		internal set { _name = value; }
	}

	public virtual string FullName {
		get { return string.Format("{0}\\{1}", Host, Name); }
		set {
			string[] splits = value.Split(new char[] { '\\' });
			if (splits.Length != 2) {
				throw new ArgumentException("Full name must be in \"HOST\\NAME\" format", "FullName");
			}
			Host = splits[0];
			Name = splits[1];
		}
	}


	public SecurityIdentifier SID { get; internal set; }

	public WinAPI.ADVAPI32.SidNameUse SidNameUsage { get; set; }


	/// <summary>
	/// Used for passing into API functions. Compatible with Windows NT+.
	/// </summary>
	protected string NTCompatibleHostName {
		get { return string.Format("\\\\{0}", Host); }
	}

	public override string ToString() {
		return Name;
	}


}
