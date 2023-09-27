﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;

namespace Hydrogen;

public abstract class ItemSerializer<TItem> : ItemSizer<TItem>, IItemSerializer<TItem> {


	protected ItemSerializer(SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) {
		SizeSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	protected SizeDescriptorSerializer SizeSerializer { get; private set; }

	public abstract void Serialize(TItem item, EndianBinaryWriter writer);

	public abstract TItem Deserialize(EndianBinaryReader reader);

	public static IItemSerializer<TItem> Default => SerializerFactory.Default.Assemble<TItem>();
		

}
