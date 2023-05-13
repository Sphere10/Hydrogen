// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hydrogen.FastReflection;

namespace Hydrogen;
public class DateTimeSerializer : StaticSizeItemSerializerBase<DateTime> {
	private readonly PrimitiveSerializer<long> _longSerializer = new();

	public DateTimeSerializer() : base(8) {
	}

	public override bool TrySerialize(DateTime item, EndianBinaryWriter writer) 
		=> _longSerializer.TrySerialize(item.ToBinary(), writer);

	public override bool TryDeserialize(EndianBinaryReader reader, out DateTime item) {
		if (!_longSerializer.TryDeserialize(reader, out var binVal)) {
			item = default;
			return false;
		}
		item = DateTime.FromBinary(binVal);
		return true;
	}
}
