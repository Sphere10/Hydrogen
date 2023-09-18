// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public abstract class ConstantLengthItemSerializerBase<TItem> : ConstantLengthItemSizer<TItem>, IAutoSizedSerializer<TItem> {
	protected ConstantLengthItemSerializerBase(long fixedSize) : base(fixedSize) {
	}

	public abstract void SerializeInternal(TItem item, EndianBinaryWriter writer);

	public TItem DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		Guard.Ensure(byteSize == ConstantLength, "Read overflow");
		return Deserialize(reader);
	}

	public abstract TItem Deserialize(EndianBinaryReader reader);

}
