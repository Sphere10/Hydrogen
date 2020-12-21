namespace Sphere10.Framework {

	public class IndexCountCallArgs : CallArgs {

		public IndexCountCallArgs(int index, int count) : base(index, count) {
		}

		public int Index { get => (int)base[0]; set => base[0]=value; }

		public int Count { get => (int)base[1]; set => base[1]=value; }
	}

}