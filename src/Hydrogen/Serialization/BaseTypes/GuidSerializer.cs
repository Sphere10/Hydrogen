// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class GuidSerializer : ConstantSizeItemSerializerBase<Guid> {
	private const int GuidByteCount = 16;
	private readonly ConstantSizeByteArraySerializer _byteArraySerializer = new(GuidByteCount);

	public GuidSerializer() : base(GuidByteCount, false) {
	}

	public static GuidSerializer Instance { get; } = new();

	public override void Serialize(Guid item, EndianBinaryWriter writer, SerializationContext context)
		=> _byteArraySerializer.Serialize(item.ToByteArray(), writer, context);

	public override Guid Deserialize(EndianBinaryReader reader, SerializationContext context)
		=> new(_byteArraySerializer.Deserialize(reader, context));

}
