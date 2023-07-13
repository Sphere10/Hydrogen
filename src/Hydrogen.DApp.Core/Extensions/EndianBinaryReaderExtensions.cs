// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.DApp.Core;

public static class EndianBinaryReaderExtensions {
	public static byte[] ReadBuffer(this EndianBinaryReader reader) {
		var len = reader.ReadUInt32();
		if (len > Int32.MaxValue)
			throw new NotSupportedException();
		return reader.ReadBytes((int)len);
	}
}
