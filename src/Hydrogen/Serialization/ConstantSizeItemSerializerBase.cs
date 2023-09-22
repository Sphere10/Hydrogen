// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public abstract class ConstantSizeItemSerializerBase<TItem> : ConstantLengthItemSizer<TItem>, IItemSerializer<TItem> {
	
	protected ConstantSizeItemSerializerBase(long fixedSize, bool supportsNull) 
		: base(fixedSize, supportsNull) {
	}

	public abstract void SerializeInternal(TItem item, EndianBinaryWriter writer);

	public abstract TItem DeserializeInternal(EndianBinaryReader reader);

}
