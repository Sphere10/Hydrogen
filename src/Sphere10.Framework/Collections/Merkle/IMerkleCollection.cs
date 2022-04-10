using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IMerkleCollection<TItem> : ICollection<TItem> {
		IMerkleTree MerkleTree { get; }

		// TODO: add proof building scope
		//  using (merkleCollection.EnterBuildProofScope()) {
		//     merkleCollection.AddRange(...);
		//     
		//  
	}

}
