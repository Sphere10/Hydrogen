using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sphere10.Framework.FastReflection;

namespace Sphere10.Framework {
	public class DateTimeOffsetSerializer : IItemSerializer<DateTimeOffset> {
		public bool IsStaticSize => true;
		public int StaticSize => _dateTimeSerializer.StaticSize + sizeof(short);

		public readonly IItemSerializer<DateTime> _dateTimeSerializer = new DateTimeSerializer();
		public int CalculateTotalSize(IEnumerable<DateTimeOffset> items, bool calculateIndividualItems, out int[] itemSizes) {
			itemSizes = Array.Empty<int>();
			var count = items.Count();

			if (calculateIndividualItems)
				itemSizes = Enumerable.Repeat(StaticSize, count).ToArray();

			return StaticSize * count;
		}

		public int CalculateSize(DateTimeOffset item) {
			return StaticSize;
		}

		public bool TrySerialize(DateTimeOffset item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				var dateTimeField = typeof(DateTimeOffset).GetField("_dateTime", BindingFlags.NonPublic | BindingFlags.Instance);
				var offsetMinutesField = typeof(DateTimeOffset).GetField("_offsetMinutes", BindingFlags.NonPublic | BindingFlags.Instance);

				DateTime dateTime = (DateTime)dateTimeField.FastGetValue(item);
				var offsetMinutes = (short)offsetMinutesField.FastGetValue(item);

				_dateTimeSerializer.Serialize(dateTime, writer);
				writer.Write(offsetMinutes);

				bytesWritten = sizeof(short) + _dateTimeSerializer.StaticSize;
				return true;
			} catch (Exception e) {
				Console.WriteLine(e);
				throw;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out DateTimeOffset item) {
			try {
				DateTime datetime = _dateTimeSerializer.Deserialize(byteSize, reader);
				short offsetMinutes = reader.ReadInt16();
				item = new DateTimeOffset(datetime.AddMinutes(offsetMinutes), TimeSpan.FromMinutes(offsetMinutes));
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}
}
