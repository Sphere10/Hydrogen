using System;
using System.ComponentModel;

namespace Hydrogen {

	public abstract class StaticSizeItemSerializerBase<TItem> : StaticSizeItemSizer<TItem>, IItemSerializer<TItem> {
		protected StaticSizeItemSerializerBase(int fixedSize) : base(fixedSize) {
		}
		
		public bool TrySerialize(TItem item, EndianBinaryWriter writer, out int bytesWritten) {
			var result = TrySerialize(item, writer);
			bytesWritten = result ? StaticSize : 0;
			return result;
		}

		public abstract bool TrySerialize(TItem item, EndianBinaryWriter writer);

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TItem item)
			=> TryDeserialize(reader, out item);

		public abstract bool TryDeserialize(EndianBinaryReader reader, out TItem item);
	}


}
