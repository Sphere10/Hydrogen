// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Hierarchy node class which contains a nested collection of hierarchy nodes
/// </summary>
/// <typeparam name="T">Entity</typeparam>
public class HierarchyNode<T> : SimpleGraph<T, HierarchyNode<T>> where T : class {
	public int Depth { get; set; }
	public T Parent { get; set; }

	public IEnumerable<HierarchyNode<T>> ChildNodes {
		get => Connections;
		set => Connections = value;
	}

}
