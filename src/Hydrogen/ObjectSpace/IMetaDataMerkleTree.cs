using System;

namespace Hydrogen.ObjectSpace.Index;

public interface IMetaDataMerkleTree : IDisposable  {

	IMerkleTree MerkleTree { get; }

}
