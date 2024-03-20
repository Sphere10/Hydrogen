using System.Collections.Generic;

namespace Hydrogen.ObjectSpaces;

public interface IObjectSpaceDimensionBuilder {
	IEnumerable<ObjectSpaceDefinition.IndexDefinition> Indexes { get; }

	IObjectSpaceDimensionBuilder WithRecyclableIndexes();

	IObjectSpaceDimensionBuilder Merkleized();

	IObjectSpaceDimensionBuilder OptimizeAssumingAverageItemSize(int bytes);

	ObjectSpaceDefinition.DimensionDefinition BuildDefinition();

	ObjectSpaceBuilder Done(); 

}
