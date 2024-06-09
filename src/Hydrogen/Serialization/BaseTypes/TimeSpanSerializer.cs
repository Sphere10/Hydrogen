// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class TimeSpanSerializer : ConstantSizeItemSerializerBase<TimeSpan> {
	private readonly PrimitiveSerializer<long> _longSerializer = new();

	public TimeSpanSerializer() : base(8, false) {
	}

	public static TimeSpanSerializer Instance { get; } = new();

	public override void Serialize(TimeSpan item, EndianBinaryWriter writer, SerializationContext context)
		=> _longSerializer.Serialize(item.Ticks, writer, context);

	public override TimeSpan Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> TimeSpan.FromTicks(_longSerializer.Deserialize(reader, context));
}
