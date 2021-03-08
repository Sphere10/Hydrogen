namespace Sphere10.Framework {

	public abstract class FixedSizeObjectSerializer<TItem> : FixedSizeObjectSizer<TItem>, IObjectSerializer<TItem> {
		protected FixedSizeObjectSerializer(int fixedSize) : base(fixedSize) {
		}

		public abstract int Serialize(TItem @object, EndianBinaryWriter writer);

		public abstract TItem Deserialize(int size, EndianBinaryReader reader);
	}

}
