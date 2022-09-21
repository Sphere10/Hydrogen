using System.Threading.Tasks;

namespace Hydrogen;

public class ContextScopeDecorator<TContextScope> : ScopeDecorator<TContextScope>, IContextScope where TContextScope : IContextScope {

	public ContextScopeDecorator(TContextScope internalScope) : base(internalScope) {
	}

	public virtual string ContextID => Internal.ContextID;

	public virtual ContextScopePolicy Policy => Internal.Policy;

	public virtual IContextScope RootScope => Internal.RootScope;

}
