// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System.Reflection;

namespace Hydrogen.Application;

[Obfuscation(Exclude = true)]
public enum HelpType : uint {
	None = 0x00000000,
	PDF = 0x00000001,
	CHM = 0x00000010,
	RTF = 0x00000100,
	URL = 0x00001000,
}
