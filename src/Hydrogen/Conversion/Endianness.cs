// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Jon Skeet, Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.
// Based from original: http://jonskeet.uk/csharp/miscutil/

namespace Hydrogen;

/// <summary>
/// Endianness of a converter
/// </summary>
public enum Endianness {
	/// <summary>
	/// Little endian - least significant byte first
	/// </summary>
	LittleEndian,

	/// <summary>
	/// Big endian - most significant byte first
	/// </summary>
	BigEndian
}
