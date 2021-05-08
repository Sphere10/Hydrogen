using System;

namespace Sphere10.Framework {

	public class ActionItemSerializer<T> : ActionItemSizer<T>, IItemSerializer<T> {
		private readonly Func<T, EndianBinaryWriter, int> _serializer;
		private readonly Func<int, EndianBinaryReader, T> _deserializer;

		public ActionItemSerializer(Func<T, int> sizer, Func<T, EndianBinaryWriter, int> serializer, Func<int, EndianBinaryReader, T> deserializer)
			: base(sizer) {
			Guard.ArgumentNotNull(serializer, nameof(serializer));
			Guard.ArgumentNotNull(deserializer, nameof(deserializer));
			_serializer = serializer;
			_deserializer = deserializer;
		}

		public int Serialize(T @object, EndianBinaryWriter writer) => _serializer(@object, writer);

		public T Deserialize(int size, EndianBinaryReader reader) => _deserializer(size, reader);
	}

}