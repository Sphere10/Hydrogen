using System.Collections.Generic;

namespace Sphere10.Framework {

	public class SimpleGraph<TNode> : ISimpleGraph<TNode>
		where TNode : ISimpleGraph<TNode> {
		public virtual IEnumerable<TNode> Connections { get; set; }
	}

	public class SimpleGraph : SimpleGraph<SimpleGraph> {
	}


	public class SimpleGraph<TEntity, TNode> : SimpleGraph<TNode>, ISimpleGraph<TEntity, TNode>
		where TNode : ISimpleGraph<TEntity, TNode> {
		public TEntity Entity { get; set; }
	}
}