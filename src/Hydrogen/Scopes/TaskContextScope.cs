using System;
using System.Threading.Tasks;

namespace Hydrogen;

public sealed class TaskContextScope : AsyncContextScope {
	private readonly Func<Task> _contextFinalizer;
	private readonly Func<Task> _scopeFinalizer;
	private readonly bool _invokeOnException;

	public TaskContextScope(Func<Task> contextFinalizer, ContextScopePolicy policy, string contextName, bool invokeOnException = true) 
		: this (contextFinalizer, default, policy, contextName, invokeOnException) {
	}

	public TaskContextScope(Func<Task> contextFinalizer, Func<Task> scopeFinalizer, ContextScopePolicy policy, string contextName, bool invokeOnException = true) : base(policy, contextName) {
		_contextFinalizer = contextFinalizer;
		_scopeFinalizer = scopeFinalizer;
		_invokeOnException = invokeOnException;
	}

	protected override async ValueTask OnScopeEndInternalAsync() {
		if (!InException || InException && _invokeOnException && _scopeFinalizer != null)
			await _scopeFinalizer();
	}

	protected override async ValueTask OnContextEndAsync() {
		if (!InException || InException && _invokeOnException && _contextFinalizer != null)
			await _contextFinalizer();
	}

}
