using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	internal class SimpleGraphAdapter<TNode, TEdge, TWeight> : ISimpleGraph 
		where TNode : IGraph<TNode, TEdge, TWeight>
		where TEdge : IGraphEdge<TNode, TEdge, TWeight>, new() {

		private readonly TNode _complexGraph;
		private readonly Func<ISimpleGraph, TEdge> _edgeCreator;

		public SimpleGraphAdapter(TNode complexGraph, Func<ISimpleGraph, TEdge> edgeCreator) {
			_complexGraph = complexGraph;
			_edgeCreator = edgeCreator;
		}

		public IEnumerable<ISimpleGraph> Connections {
			get {
				return (IEnumerable<ISimpleGraph>)_complexGraph.Edges.Select(edge => new SimpleGraphAdapter<TNode, TEdge, TWeight>(edge.Destination, _edgeCreator) );
			}
			set {
				if (_edgeCreator != null)
					_complexGraph.Edges = (value ?? Enumerable.Empty<ISimpleGraph>()).Select(node => _edgeCreator(node));
			}
		}
	}

}