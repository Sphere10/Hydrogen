using System;
using System.Threading.Tasks;

namespace Hydrogen;

public sealed class TaskContextScope : AsyncContextScope {
	private readonly Func<Task> _contextFinalizer;
	private readonly Func<Task> _scopeFinalizer;


	public TaskContextScope(Func<Task> contextFinalizer, ContextScopePolicy policy, string contextName)
		: this(contextFinalizer, default, policy, contextName) {
	}

	public TaskContextScope(Func<Task> contextFinalizer, Func<Task> scopeFinalizer, ContextScopePolicy policy, string contextName) : base(policy, contextName) {
		_contextFinalizer = contextFinalizer;
		_scopeFinalizer = scopeFinalizer;
	}

	protected override async ValueTask OnScopeEndInternalAsync() {
		if (_scopeFinalizer != null)
			await _scopeFinalizer();
	}

	protected override async ValueTask OnContextEndAsync() {
		if (_contextFinalizer != null)
			await _contextFinalizer();
	}

}
