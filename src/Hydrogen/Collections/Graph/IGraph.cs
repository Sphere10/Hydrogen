// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

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
