// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ConstantSizeByteArraySerializer : ConstantSizeItemSerializerBase<byte[]> {

	public ConstantSizeByteArraySerializer(int size) : base(size, false) {
	}

	public override void Serialize(byte[] item, EndianBinaryWriter writer) {
		Guard.ArgumentNotNull(item, nameof(item));
		Guard.Argument(item.Length == ConstantSize, nameof(item), "Incorrectly sized");
		writer.Write(item);
	}

	public override byte[] Deserialize(EndianBinaryReader reader)
		=> reader.ReadBytes(ConstantSize);
}
