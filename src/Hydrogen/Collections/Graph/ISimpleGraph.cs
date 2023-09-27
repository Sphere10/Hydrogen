// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

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
