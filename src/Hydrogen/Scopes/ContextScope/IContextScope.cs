using System.Threading.Tasks;

namespace Hydrogen;

public interface IContextScope : IScope {

	string ContextID { get; }

	ContextScopePolicy Policy { get; }

	IContextScope RootScope { get; }

}
