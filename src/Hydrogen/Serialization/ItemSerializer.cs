// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;

namespace Hydrogen;

public abstract class ItemSerializer<TItem> : ItemSizer<TItem>, IItemSerializer<TItem> {
	public abstract bool TrySerialize(TItem item, EndianBinaryWriter writer, out long bytesWritten);

	public abstract bool TryDeserialize(long byteSize, EndianBinaryReader reader, out TItem item);

	public static IItemSerializer<TItem> Default {
		get {
			var type = typeof(TItem);

			if (Tools.Memory.IsSerializationPrimitive(type))
				return new PrimitiveSerializer<TItem>();

			if (type == typeof(string))
				return new StringSerializer(Encoding.UTF8) as IItemSerializer<TItem>;

			return new GenericSerializer<TItem>();
		}
	}
}
