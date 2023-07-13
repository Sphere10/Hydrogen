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
public sealed class AssemblyAuthorAttribute : Attribute {

	public AssemblyAuthorAttribute(string name) : this(name, null) {
	}

	public AssemblyAuthorAttribute(string name, string email) {
		Name = name;
		Email = email;
	}

	public string Name { get; init; }

	public string Email { get; init; }

}
