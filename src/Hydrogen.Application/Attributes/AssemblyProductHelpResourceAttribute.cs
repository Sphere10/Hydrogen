// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Application;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public abstract class AssemblyProductHelpResourceAttribute : Attribute {

	public AssemblyProductHelpResourceAttribute(HelpType helpType, string path) {
		HelpType = helpType;
		Path = path;
	}

	public HelpType HelpType { get; set; }

	public string Path { get; set; }


}
