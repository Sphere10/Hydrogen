using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sphere10.Framework {

	/// <summary>
	/// A serializer that uses the .NET <see cref="BinaryFormatter"/> under the hood.
	/// </summary>
	/// <remarks>Due to limitations of <see cref="BinaryFormatter"/> this class performs a serialization on <see cref="CalculateSize"/>.</remarks>
	/// <typeparam name="TItem"></typeparam>
	public sealed class BinaryFormattedSerializer<TItem> : IItemSerializer<TItem> {

		public bool IsStaticSize => false;
		public int StaticSize => -1;

		public int CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out int[] itemSizes) {
			var itemsArr = items as TItem[] ?? items.ToArray();

			if (calculateIndividualItems) {
				itemSizes = new int[itemsArr.Length];
				for (int i = 0; i < itemsArr.Length; i++) {
					itemSizes[i] = CalculateSize(itemsArr[i]);
				}
			}

			int sum = 0;
			for (int i = 0; i < itemsArr.Length; i++) {
				sum += CalculateSize(itemsArr[i]);
			}
			itemSizes = Array.Empty<int>();
			return sum;
		}

		public int CalculateSize(TItem item) {
			var formatter = new BinaryFormatter();
			using var memoryStream = new MemoryStream();
			formatter.Serialize(memoryStream, item);
			var objectRawBytes = memoryStream.ToArray();
			return objectRawBytes.Length;
		}

		public bool TrySerialize(TItem item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				var formatter = new BinaryFormatter();
				using var memoryStream = new MemoryStream();
				formatter.Serialize(memoryStream, item);
				var objectRawBytes = memoryStream.ToArray();
				writer.Write(objectRawBytes);
				bytesWritten = objectRawBytes.Length;

				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TItem item) {
			try {
				var bytes = reader.ReadBytes(byteSize);
				var formatter = new BinaryFormatter();
				using var memoryStream = new MemoryStream(bytes);
				var result = formatter.Deserialize(memoryStream);
				if (result is null)
					throw new InvalidOperationException("Unexpected null");
				if (result is not TItem t)
					throw new InvalidOperationException($"Unexpected type '{result.GetType().Name}'");

				item = t;
				return true;

			} catch (Exception) {
				item = default;
				return false;
			}
		}

	}
}
