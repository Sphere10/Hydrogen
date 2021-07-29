using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sphere10.Framework.FastReflection;

namespace Sphere10.Framework {
	public class DateTimeSerializer : IItemSerializer<DateTime> {

		public bool IsFixedSize => true;

		public int FixedSize => sizeof(ulong);

		public int CalculateTotalSize(IEnumerable<DateTime> items, bool calculateIndividualItems, out int[] itemSizes) {
			itemSizes = Array.Empty<int>();
			var count = items.Count();

			if (calculateIndividualItems)
				itemSizes = Enumerable.Repeat(FixedSize, count).ToArray();

			return FixedSize * count;
		}

		public int CalculateSize(DateTime item) {
			return sizeof(long);
		}

		public bool TrySerialize(DateTime item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				var property = typeof(DateTime).GetProperty("Ticks", BindingFlags.Public | BindingFlags.Instance);
				var dateData = (long)property.FastGetValue(item);
				byte[] bytes = writer.BitConverter.GetBytes(dateData);
				writer.Write(bytes);

				bytesWritten = bytes.Length;
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out DateTime item) {
			try {
				long ticks = reader.ReadInt64();
				item = new DateTime(ticks);
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}


	public class DateTimeOffsetSerializer : IItemSerializer<DateTimeOffset> {
		public bool IsFixedSize => true;
		public int FixedSize => _dateTimeSerializer.FixedSize + sizeof(short);

		public readonly IItemSerializer<DateTime> _dateTimeSerializer = new DateTimeSerializer();
		public int CalculateTotalSize(IEnumerable<DateTimeOffset> items, bool calculateIndividualItems, out int[] itemSizes) {
			itemSizes = Array.Empty<int>();
			var count = items.Count();

			if (calculateIndividualItems)
				itemSizes = Enumerable.Repeat(FixedSize, count).ToArray();

			return FixedSize * count;
		}

		public int CalculateSize(DateTimeOffset item) {
			return FixedSize;
		}

		public bool TrySerialize(DateTimeOffset item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				var dateTimeField = typeof(DateTimeOffset).GetField("_dateTime", BindingFlags.NonPublic | BindingFlags.Instance);
				var offsetMinutesField = typeof(DateTimeOffset).GetField("_offsetMinutes", BindingFlags.NonPublic | BindingFlags.Instance);

				DateTime dateTime = (DateTime)dateTimeField.FastGetValue(item);
				var offsetMinutes = (short)offsetMinutesField.FastGetValue(item);

				_dateTimeSerializer.Serialize(dateTime, writer);
				writer.Write(offsetMinutes);

				bytesWritten = sizeof(short) + _dateTimeSerializer.FixedSize;
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
