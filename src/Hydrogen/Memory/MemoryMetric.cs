// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;

namespace Hydrogen;

public enum MemoryMetric {
	[Description("bits")] Bit = 0,

	[Description("kbits")] Kilobit,

	[Description("mbits")] Megabit,

	[Description("gbits")] Gigabit,

	[Description("tbits")] Terrabit,

	[Description("pbits")] Petabit,

	[Description("b")] Byte,

	[Description("kb")] Kilobyte,

	[Description("mb")] Megabyte,

	[Description("gb")] Gigabyte,

	[Description("tb")] Terrabyte,

	[Description("pb")] PetaByte
}
