// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

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
