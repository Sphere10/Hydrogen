//-----------------------------------------------------------------------
// <copyright file="HierarchyNode.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

	/// <summary>
	/// Hierarchy node class which contains a nested collection of hierarchy nodes
	/// </summary>
	/// <typeparam name="T">Entity</typeparam>
	public class HierarchyNode<T> : SimpleGraph<T,  HierarchyNode<T>> where T : class {
		public int Depth { get; set; }
		public T Parent { get; set; }
		public IEnumerable<HierarchyNode<T>> ChildNodes { 
			get => Connections;
			set => Connections = value;
		}

	}


}
