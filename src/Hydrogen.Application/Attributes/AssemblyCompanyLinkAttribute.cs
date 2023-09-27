// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.InteropServices;

namespace Hydrogen.Application;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class AssemblyCompanyLinkAttribute : Attribute {
	private string _companyLink = null;

	public AssemblyCompanyLinkAttribute(string url) {
		CompanyLink = url;
	}

	public string CompanyLink {
		get { return _companyLink; }
		set { _companyLink = value; }
	}
}
