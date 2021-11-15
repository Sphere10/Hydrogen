using System.IO;

namespace Sphere10.Framework {

	internal abstract class StreamPageBase<TItem> : PageBase<TItem> {
		protected StreamPageBase(StreamPagedList<TItem> parent) {
			Parent = parent;
		}

		public long StartPosition { get; protected set; }
		
		public abstract int ReadItemBytes(int itemIndex, int byteOffset, int byteLength, out byte[] bytes);

		protected StreamPagedList<TItem> Parent { get; }

		protected Stream Stream => Parent.Stream;

		protected EndianBinaryReader Reader => Parent.Reader;

		protected int ItemSize => Parent.Serializer.FixedSize;

		protected IItemSerializer<TItem> Serializer => Parent.Serializer;

		protected EndianBinaryWriter Writer => Parent.Writer;
	}

}
