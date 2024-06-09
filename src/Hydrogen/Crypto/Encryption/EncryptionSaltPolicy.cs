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
public enum EncryptionSaltPolicy {
	None = 1,
	MACAddress = 1 << 1,
	ExecutableFileCreationTime = 1 << 2,
	OperatingSystemVersion = 1 << 3,
	MachineName = 1 << 4,
	UserName = 1 << 5,
	ProcessorCount = 1 << 6,
	Custom = 1 << 7
}
