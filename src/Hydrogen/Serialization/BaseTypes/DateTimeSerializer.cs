// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class DateTimeSerializer : ConstantSizeItemSerializerBase<DateTime> {
	private readonly PrimitiveSerializer<long> _longSerializer = new();

	public static DateTimeSerializer Instance { get; } = new();

	public DateTimeSerializer() : base(8, false) {
	}

	public override void Serialize(DateTime item, EndianBinaryWriter writer, SerializationContext context)
		=> _longSerializer.Serialize(item.ToBinary(), writer, context);

	public override DateTime Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> DateTime.FromBinary(_longSerializer.Deserialize(reader, context));
}
