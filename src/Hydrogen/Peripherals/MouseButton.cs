// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Reflection;

namespace Hydrogen;

[Obfuscation(Exclude = true)]
[Flags]
public enum MouseButton {
	None = 0,
	Left,
	Right,
	Middle,
	XButton1,
	XButton2
}
