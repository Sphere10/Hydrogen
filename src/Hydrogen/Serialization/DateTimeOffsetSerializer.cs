// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class DateTimeOffsetSerializer : ConstantSizeItemSerializerBase<DateTimeOffset> {

	public DateTimeOffsetSerializer() 
		: base(DateTimeSerializer.Instance.ConstantSize + TimeSpanSerializer.Instance.ConstantSize, false){
	}

	public static DateTimeOffsetSerializer Instance { get; } = new();

	public override void SerializeInternal(DateTimeOffset item, EndianBinaryWriter writer) {
		DateTimeSerializer.Instance.SerializeInternal(item.LocalDateTime, writer);
		TimeSpanSerializer.Instance.SerializeInternal(item.Offset, writer);
	}

	public override DateTimeOffset DeserializeInternal(EndianBinaryReader reader) {
		var datetime = DateTimeSerializer.Instance.Deserialize(reader);
		var timespan = TimeSpanSerializer.Instance.Deserialize(reader);
		return new DateTimeOffset(datetime, timespan);
	}
}
