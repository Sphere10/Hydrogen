// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Reflection;

namespace Hydrogen;

/// <summary>
/// Summary description for MouseClickType.
/// </summary>
[Obfuscation(Exclude = true)]
public enum MouseClickType {
	None = 0,
	Single,
	Double,
	Tripple
}
