// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

[Flags]
public enum EventTraits : uint {
	Access = 1 << 0,
	Read = 1 << 1 | Access,
	Write = 1 << 2 | Access,
	Count = 1 << 3 | Read,
	Search = 1 << 4 | Read,
	Fetch = 1 << 5 | Read,
	Add = 1 << 6 | Write,
	Update = 1 << 7 | Write,
	Insert = 1 << 8 | Write,
	Remove = 1 << 9 | Write,


	All = uint.MaxValue,
	None = 0,
}
