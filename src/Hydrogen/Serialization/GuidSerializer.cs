// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class GuidSerializer : StaticSizeItemSerializerBase<Guid> {
	private const int GuidByteCount = 16;
	private readonly StaticSizeByteArraySerializer _byteArraySerializer = new(GuidByteCount);
	public GuidSerializer() : base(GuidByteCount) {
	}

	public override bool TrySerialize(Guid item, EndianBinaryWriter writer)
		=> _byteArraySerializer.TrySerialize(item.ToByteArray(), writer);

	public override bool TryDeserialize(EndianBinaryReader reader, out Guid item) {
		if (_byteArraySerializer.TryDeserialize(reader, out var bytes)) {
			item = new Guid(bytes);
			return true;
		}
		item = default;
		return false;
	}

}
