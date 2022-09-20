using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Hydrogen;

public abstract class SyncScopeBase : ScopeBase {

	protected sealed override async ValueTask OnScopeEndAsync() => OnScopeEnd();


}
