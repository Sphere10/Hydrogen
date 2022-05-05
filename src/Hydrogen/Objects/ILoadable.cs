namespace Hydrogen {

	public interface ILoadable {
		event EventHandlerEx<object> Loading;
		event EventHandlerEx<object> Loaded;
		bool RequiresLoad { get; }
		void Load();
	}

}
