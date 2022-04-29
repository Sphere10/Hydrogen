//-----------------------------------------------------------------------
// <copyright file="GraphNode.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Sphere10.Framework {



	public interface IGraphEdge<TNode, TEdge, TWeight>
		where TNode : IGraph<TNode, TEdge, TWeight>
		where TEdge : IGraphEdge<TNode, TEdge, TWeight>, new() {
		TWeight Weight { get; set; }
		TNode Source { get; set; }
		TNode Destination { get; set; }
	}

	public interface IGraphEdge<TNode> : IGraphEdge<TNode, GraphEdge<TNode>, int>
		where TNode : IGraph<TNode> {
	}

	public interface IGraphEdge : IGraphEdge<Graph> {
	}

	public class Graph<TNode, TEdge, TWeight> : IGraph<TNode, TEdge, TWeight>
		where TNode : IGraph<TNode, TEdge, TWeight>
		where TEdge : IGraphEdge<TNode, TEdge, TWeight>, new() {


		public virtual IEnumerable<TEdge> Edges { get; set; }

		public ISimpleGraph ToSimpleGraph(Func<ISimpleGraph, TEdge> edgeCreator = null) {
			return new SimpleGraphAdapter<TNode,TEdge,TWeight>((TNode)(object)this, edgeCreator);
		}

	}

	public class Graph<TNode> : Graph<TNode, GraphEdge<TNode>, int>, IGraph<TNode>
		where TNode : IGraph<TNode> {
	}

	public class Graph : Graph<Graph>, IGraph {
	}

	public class GraphEdge<TNode, TEdge, TWeight> : IGraphEdge<TNode, TEdge, TWeight>
		where TNode : IGraph<TNode, TEdge, TWeight>
		where TEdge : IGraphEdge<TNode, TEdge, TWeight>, new() {

		public virtual TWeight Weight { get; set; }
		public virtual TNode Source { get; set; }
		public virtual TNode Destination { get; set; }
	}

	public class GraphEdge<TNode> : GraphEdge<TNode, GraphEdge<TNode>, int>, IGraphEdge<TNode>
		where TNode : IGraph<TNode> {
	}

	public class GraphEdge : GraphEdge<Graph>, IGraphEdge {
	}

}
