using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Hydrogen;

public abstract class SyncScope : ScopeBase {

	protected sealed override async ValueTask OnScopeEndAsync() => await Task.Run(OnScopeEnd);


}
