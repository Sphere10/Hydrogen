// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public abstract class ConstantSizeItemSerializerBase<TItem> : ConstantLengthItemSizer<TItem>, IItemSerializer<TItem> {
	
	protected ConstantSizeItemSerializerBase(long fixedSize, bool supportsNull) 
		: base(fixedSize, supportsNull) {
	}

	public abstract void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context);

	public abstract TItem Deserialize(EndianBinaryReader reader, SerializationContext context);

}
