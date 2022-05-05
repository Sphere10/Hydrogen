namespace Hydrogen;

/// <summary>
/// A merkleized object is one that maintains a merkle tree of it's state.
/// </summary>
public interface IMerkleObject {
	IMerkleTree MerkleTree { get; }

	// TODO: add proof building scope
	//  using (merkleCollection.EnterBuildProofScope()) {
	//     merkleCollection.AddRange(...);
	//     
	//  
}
