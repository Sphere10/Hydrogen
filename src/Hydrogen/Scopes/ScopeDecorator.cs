using System.Threading.Tasks;

namespace Hydrogen;

public class ScopeDecorator<TScope> : IScope where TScope : IScope {
	public event EventHandlerEx ScopeEnd;

	protected internal TScope Internal;

	public ScopeDecorator(TScope internalScope) {
		Guard.ArgumentNotNull(internalScope, nameof(internalScope));
		Internal = internalScope;
	}

	public virtual void Dispose() => Internal.Dispose();

	public virtual ValueTask DisposeAsync() => Internal.DisposeAsync();
	
}
