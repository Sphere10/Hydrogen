using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Hydrogen {

	public abstract class ItemSerializer<TItem> : ItemSizer<TItem>, IItemSerializer<TItem> {
		public abstract bool TrySerialize(TItem item, EndianBinaryWriter writer, out int bytesWritten);

		public abstract bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TItem item);

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


}