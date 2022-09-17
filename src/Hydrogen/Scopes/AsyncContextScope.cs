using System;
using System.Threading.Tasks;

namespace Hydrogen;

public sealed class AsyncContextScope : AsyncContextScopeBase<AsyncContextScope> {
	private readonly Func<Task> _rootScopeFinalizer;
	private readonly Func<Task> _thisScopeFinalizer;
	private readonly bool _invokeOnException;

	public AsyncContextScope(Func<Task> rootScopeFinalizer, Func<Task> thisScopeFinalizer, ContextScopePolicy policy, string contextName, bool invokeOnException = true) : base(policy, contextName) {
		_rootScopeFinalizer = rootScopeFinalizer;
		_thisScopeFinalizer = thisScopeFinalizer;
		_invokeOnException = invokeOnException;
	}

	protected override async ValueTask OnScopeEndAsync(AsyncContextScope rootScope, bool inException) {
		if (!inException || inException && _invokeOnException) {
			if (_thisScopeFinalizer != null)
				await _thisScopeFinalizer();

			if (IsRootScope &&  _rootScopeFinalizer != null)
				await _rootScopeFinalizer();
		}
	}
}
