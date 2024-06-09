// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

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
		get { return (IEnumerable<ISimpleGraph>)_complexGraph.Edges.Select(edge => new SimpleGraphAdapter<TNode, TEdge, TWeight>(edge.Destination, _edgeCreator)); }
		set {
			if (_edgeCreator != null)
				_complexGraph.Edges = (value ?? Enumerable.Empty<ISimpleGraph>()).Select(node => _edgeCreator(node));
		}
	}
}
