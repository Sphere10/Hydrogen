using System;
using System.IO;

namespace Sphere10.Framework {
	public interface IItemSerializer<TItem> : IItemSizer<TItem> {

		bool TrySerialize(TItem item, EndianBinaryWriter writer, out int bytesWritten);

		bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TItem item);

	}
}