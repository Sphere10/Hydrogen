namespace Sphere10.Framework {

	public abstract class ConstantObjectSerializer<TItem> : ConstantObjectSizer<TItem>, IObjectSerializer<TItem> {
		protected ConstantObjectSerializer(int fixedSize) : base(fixedSize) {
		}

		public abstract int Serialize(TItem @object, EndianBinaryWriter writer);

		public abstract TItem Deserialize(int size, EndianBinaryReader reader);
	}

}
