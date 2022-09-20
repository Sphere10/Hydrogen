using System;
using System.Threading.Tasks;

namespace Hydrogen;

public sealed class AsyncContextScope : AsyncContextScopeBase<AsyncContextScope> {
	private readonly Func<Task> _contextFinalizer;
	private readonly Func<Task> _scopeFinalizer;
	private readonly bool _invokeOnException;

	public AsyncContextScope(Func<Task> contextFinalizer, Func<Task> scopeFinalizer, ContextScopePolicy policy, string contextName, bool invokeOnException = true) : base(policy, contextName) {
		_contextFinalizer = contextFinalizer;
		_scopeFinalizer = scopeFinalizer;
		_invokeOnException = invokeOnException;
	}

	protected override async ValueTask OnScopeEndAsync(AsyncContextScope rootScope, bool inException) {
		if (!inException || inException && _invokeOnException) {
			if (_scopeFinalizer != null)
				await _scopeFinalizer();

			if (IsRootScope && _contextFinalizer != null)
				await _contextFinalizer();
		}
	}
}
