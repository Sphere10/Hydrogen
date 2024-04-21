// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.ObjectSpaces;

public interface IObjectSpaceDimensionBuilder {

	Type ItemType { get; }

	IEnumerable<ObjectSpaceDefinition.IndexDefinition> Indexes { get; }

	IObjectSpaceDimensionBuilder WithRecyclableIndexes();

	IObjectSpaceDimensionBuilder Merkleized();

	IObjectSpaceDimensionBuilder OptimizeAssumingAverageItemSize(int bytes);

	ObjectSpaceDefinition.DimensionDefinition BuildDefinition();

	ObjectSpaceBuilder Done(); 

}
