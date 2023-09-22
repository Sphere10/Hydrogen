// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class TimeSpanSerializer : ConstantSizeItemSerializerBase<TimeSpan> {
	private readonly PrimitiveSerializer<long> _longSerializer = new();

	public TimeSpanSerializer() : base(8, false) {
	}

	public static TimeSpanSerializer Instance { get; } = new();

	public override void SerializeInternal(TimeSpan item, EndianBinaryWriter writer)
		=> _longSerializer.SerializeInternal(item.Ticks, writer);

	public override TimeSpan DeserializeInternal(EndianBinaryReader reader) 
		=> TimeSpan.FromTicks(_longSerializer.Deserialize(reader));
}
