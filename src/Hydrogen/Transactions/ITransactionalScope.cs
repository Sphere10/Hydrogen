using System.Data;
using System.Threading.Tasks;

namespace Hydrogen;

public interface ITransactionalScope : IContextScope, ITransactionalObject {

	new ITransactionalScope RootScope { get; }

	void BeginTransaction();

	Task BeginTransactionAsync();

}
