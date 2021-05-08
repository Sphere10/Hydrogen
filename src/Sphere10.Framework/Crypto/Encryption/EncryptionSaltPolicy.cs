//-----------------------------------------------------------------------
// <copyright file="EncryptionSaltPolicy.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sphere10.Framework {
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
}
