// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Runtime.InteropServices;
using System;

namespace Hydrogen.Application;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class AssemblyCompanyNumberAttribute : Attribute {

	public AssemblyCompanyNumberAttribute(string companyNumber) {
		CompanyNumber = companyNumber;
	}

	public string CompanyNumber { get; set; }
}
