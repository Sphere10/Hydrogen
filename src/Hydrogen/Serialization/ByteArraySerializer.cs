// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ByteArraySerializer : ItemSerializer<byte[]> {


	public ByteArraySerializer(SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(sizeDescriptorStrategy) {
	}

	public static ByteArraySerializer Instance { get; } = new();

	public override long CalculateSize(byte[] item) => item.Length;

	public override void Serialize(byte[] item, EndianBinaryWriter writer) {
		SizeSerializer.Serialize(item.Length, writer);
		writer.Write(item);
	}

	public override byte[] Deserialize(EndianBinaryReader reader) {
		var byteSize = SizeSerializer.Deserialize(reader);
		 return reader.ReadBytes(byteSize);
	}

}
