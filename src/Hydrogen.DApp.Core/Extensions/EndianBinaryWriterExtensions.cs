// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.DApp.Core;

public static class EndianBinaryWriterExtensions {
	public static void WriteBuffer(this EndianBinaryWriter writer, byte[] buffer) {
		Guard.ArgumentNotNull(buffer, nameof(buffer));
		Guard.ArgumentInRange(buffer.Length, 0, UInt32.MaxValue, nameof(buffer.Length));
		writer.Write(buffer.Length);
		writer.Write(buffer);
	}
}
