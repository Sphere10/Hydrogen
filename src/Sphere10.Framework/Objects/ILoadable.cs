namespace Sphere10.Framework {

	public interface ILoadable {
		bool RequiresLoad { get; }
		void Load();
	}

}
