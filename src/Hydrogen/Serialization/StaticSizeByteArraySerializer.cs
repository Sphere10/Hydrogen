// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen {

	public class StaticSizeByteArraySerializer : StaticSizeItemSerializerBase<byte[]> {

		public StaticSizeByteArraySerializer(int size) : base(size) {
		}

		public override bool TrySerialize(byte[] item, EndianBinaryWriter writer) {
			Guard.Ensure(item.Length == StaticSize, "Incorrectly sized");
			writer.Write(item);
			return true;
		}

		public override bool TryDeserialize(EndianBinaryReader reader, out byte[] item) {
			item = reader.ReadBytes(StaticSize);
			return true;
		}
	}
}
