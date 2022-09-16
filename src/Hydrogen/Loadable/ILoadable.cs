using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen;

public interface ILoadable {
	event EventHandlerEx<object> Loading;
	event EventHandlerEx<object> Loaded;
	bool RequiresLoad { get; }
	void Load();
	Task LoadAsync();
}