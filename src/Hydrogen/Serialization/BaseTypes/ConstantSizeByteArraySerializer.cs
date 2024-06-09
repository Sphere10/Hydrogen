// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ConstantSizeByteArraySerializer : ConstantSizeItemSerializerBase<byte[]> {

	public ConstantSizeByteArraySerializer(int size) : base(size, false) {
	}

	public override void Serialize(byte[] item, EndianBinaryWriter writer, SerializationContext context) {
		Guard.ArgumentNotNull(item, nameof(item));
		Guard.Argument(item.Length == ConstantSize, nameof(item), "Incorrectly sized");
		writer.Write(item);
	}

	public override byte[] Deserialize(EndianBinaryReader reader, SerializationContext context)
		=> reader.ReadBytes(ConstantSize);
}
