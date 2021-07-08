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
			return sizeof(ulong);
		}

		public int Serialize(DateTime @object, EndianBinaryWriter writer) {
			var property = typeof(DateTime).GetProperty("Ticks", BindingFlags.Public | BindingFlags.Instance);
			var dateData = (long)property.FastGetValue(@object);
			byte[] bytes = writer.BitConverter.GetBytes(dateData);
			writer.Write(bytes);
			
			return bytes.Length;
		}

		public DateTime Deserialize(int size, EndianBinaryReader reader) {
			long ticks = reader.ReadInt64();
			return new DateTime(ticks);
		}
	}
	
	public class DateTimeOffsetSerializer : IItemSerializer<DateTimeOffset> {
		public bool IsFixedSize => true;
		public int FixedSize => _dateTimeSerializer.FixedSize + sizeof(int);

		public readonly IItemSerializer<DateTime> _dateTimeSerializer = new DateTimeSerializer();
		public int CalculateTotalSize(IEnumerable<DateTimeOffset> items, bool calculateIndividualItems, out int[] itemSizes) {
			itemSizes = Array.Empty<int>();
			var count = items.Count();
				
			if (calculateIndividualItems)
				itemSizes = Enumerable.Repeat(FixedSize, count).ToArray();

			return FixedSize * count;
		}

		public int CalculateSize(DateTimeOffset item) {
			throw new NotImplementedException();
		}

		public int Serialize(DateTimeOffset @object, EndianBinaryWriter writer) {
			var dateTimeField = typeof(DateTimeOffset).GetField("_dateTime", BindingFlags.NonPublic | BindingFlags.Instance);
			var offsetMinutesField = typeof(DateTimeOffset).GetField("_offsetMinutes", BindingFlags.NonPublic | BindingFlags.Instance);

			DateTime dateTime = (DateTime)dateTimeField.FastGetValue(@object);
			short offsetMinutes = (short)offsetMinutesField.FastGetValue(@object);
			
			_dateTimeSerializer.Serialize(dateTime, writer);
			writer.Write(offsetMinutes);

			return sizeof(int) + _dateTimeSerializer.FixedSize;
		}

		public DateTimeOffset Deserialize(int size, EndianBinaryReader reader) {
			DateTime datetime = _dateTimeSerializer.Deserialize(size, reader);
			short offsetMinutes = reader.ReadInt16();
			return new DateTimeOffset(datetime.AddMinutes(offsetMinutes), TimeSpan.FromMinutes(offsetMinutes));
		}
	}
}
