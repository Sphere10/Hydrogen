using System.Threading.Tasks;

namespace Hydrogen;

public interface IAsyncLoadable {
	event EventHandlerEx<object> Loading;
	event EventHandlerEx<object> Loaded;
	bool RequiresLoad { get; }
	Task Load();
}