// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class DateTimeOffsetSerializer : ConstantSizeItemSerializerBase<DateTimeOffset> {

	public DateTimeOffsetSerializer() 
		: base(DateTimeSerializer.Instance.ConstantSize + TimeSpanSerializer.Instance.ConstantSize, false){
	}

	public static DateTimeOffsetSerializer Instance { get; } = new();

	public override void Serialize(DateTimeOffset item, EndianBinaryWriter writer, SerializationContext context) {
		DateTimeSerializer.Instance.Serialize(item.LocalDateTime, writer, context);
		TimeSpanSerializer.Instance.Serialize(item.Offset, writer, context);
	}

	public override DateTimeOffset Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var datetime = DateTimeSerializer.Instance.Deserialize(reader, context);
		var timespan = TimeSpanSerializer.Instance.Deserialize(reader, context);
		return new DateTimeOffset(datetime, timespan);
	}
}
