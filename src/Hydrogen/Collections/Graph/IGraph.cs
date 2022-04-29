using System;
using System.Collections.Generic;

namespace Hydrogen {

	public interface IGraph<TNode, TEdge, TWeight>
		where TNode : IGraph<TNode, TEdge, TWeight>
		where TEdge : IGraphEdge<TNode, TEdge, TWeight>, new() {

		IEnumerable<TEdge> Edges { get; set; }

		ISimpleGraph ToSimpleGraph(Func<ISimpleGraph, TEdge> edgeCreator = null);
	}

	public interface IGraph<TNode> : IGraph<TNode, GraphEdge<TNode>, int>
		where TNode : IGraph<TNode> {
	}

	public interface IGraph : IGraph<Graph> {
	}

}