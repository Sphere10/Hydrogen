using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sphere10.Framework {

	/// <summary>
	/// A serializer that uses the .NET <see cref="BinaryFormatter"/> under the hood.
	/// </summary>
	/// <remarks>Due to limitations of <see cref="BinaryFormatter"/> this class performs a serialization on <see cref="CalculateSize"/>.</remarks>
	/// <typeparam name="TItem"></typeparam>
	public class BinaryFormattedSerializer<TItem> : ItemSerializerBase<TItem> {

		public override int CalculateSize(TItem item) {
			var formatter = new BinaryFormatter();
			using var memoryStream = new MemoryStream();
			formatter.Serialize(memoryStream, item);
			var objectRawBytes = memoryStream.ToArray();
			return objectRawBytes.Length;
		}

		public override TItem Deserialize(int size, EndianBinaryReader reader) {
			var bytes = reader.ReadBytes(size);
			var formatter = new BinaryFormatter();
			using var memoryStream = new MemoryStream(bytes);
			var result = formatter.Deserialize(memoryStream);
			if (result is null)
				throw new InvalidOperationException("Unexpected null");
			if (result is not TItem item) 
				throw new InvalidOperationException($"Unexpected type '{result.GetType().Name}'");
			return item;
		}

		public override int Serialize(TItem item, EndianBinaryWriter writer) {
			var formatter = new BinaryFormatter();
			using var memoryStream = new MemoryStream();
			formatter.Serialize(memoryStream, item);
			var objectRawBytes = memoryStream.ToArray();
			writer.Write(objectRawBytes);
			return objectRawBytes.Length;
		}
	}
}
