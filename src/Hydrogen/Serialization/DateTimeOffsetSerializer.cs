// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class DateTimeOffsetSerializer : ConstantLengthItemSerializerBase<DateTimeOffset> {

	public DateTimeOffsetSerializer() 
		: base(DateTimeSerializer.Instance.ConstantLength + PrimitiveSerializer<short>.Instance.ConstantLength, false){
	}

	public override void SerializeInternal(DateTimeOffset item, EndianBinaryWriter writer) {
		DateTimeSerializer.Instance.SerializeInternal(item.LocalDateTime, writer);
		PrimitiveSerializer<short>.Instance.SerializeInternal((short)item.Offset.TotalMinutes, writer);
	}

	public override DateTimeOffset Deserialize(EndianBinaryReader reader) {
		var datetime = DateTimeSerializer.Instance.Deserialize(reader);
		var offsetMinutes = PrimitiveSerializer<short>.Instance.Deserialize(reader);
		return new DateTimeOffset(datetime, TimeSpan.FromMinutes(offsetMinutes));
	}
}
