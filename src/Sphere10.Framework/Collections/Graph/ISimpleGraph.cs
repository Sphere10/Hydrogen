using System.Collections.Generic;

namespace Sphere10.Framework {

	// specialization
	public interface ISimpleGraph<TEntity, TNode> : ISimpleGraph<TNode>
		where TNode : ISimpleGraph<TEntity, TNode> {
		TEntity Entity { get; set; }
	}

	public interface ISimpleGraph<TNode> 
		where TNode : ISimpleGraph<TNode> {
		IEnumerable<TNode> Connections { get; set; }
	}

	public interface ISimpleGraph : ISimpleGraph<ISimpleGraph> {
	}

}