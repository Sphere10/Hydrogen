using System.Threading.Tasks;

namespace Hydrogen;

public class NoOpLoadable : ILoadable {
	public event EventHandlerEx<object> Loading;
	public event EventHandlerEx<object> Loaded;
	public bool RequiresLoad => false;

	public static readonly NoOpLoadable Instance = new();

	public void Load() {
	}

	public Task LoadAsync() => Task.CompletedTask;
}
