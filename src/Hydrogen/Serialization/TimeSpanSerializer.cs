// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class TimeSpanSerializer : ConstantLengthItemSerializerBase<TimeSpan> {
	private readonly PrimitiveSerializer<long> _longSerializer = new();

	public static TimeSpanSerializer Instance { get; } = new();

	public TimeSpanSerializer() : base(8, false) {
	}

	public override void SerializeInternal(TimeSpan item, EndianBinaryWriter writer)
		=> _longSerializer.SerializeInternal(item.Ticks, writer);

	public override TimeSpan Deserialize(EndianBinaryReader reader) 
		=> TimeSpan.FromTicks(_longSerializer.Deserialize(reader));
}
