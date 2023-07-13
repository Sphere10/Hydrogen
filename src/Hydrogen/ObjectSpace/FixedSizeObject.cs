// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//public interface IConsensusObject {
//	public long ID { get; }

//	public int TypeCode { get; }

//	public int Index { get; }

//	public void Load();

//	public void Save();

//  public byte[] Serialized { get; }

//  public IConsensusSpace Space { get; }

//}


////using System;
////using System.Collections.Generic;

////namespace Hydrogen.ObjectSpace {

////	// transient properties are serialized but not merkleized
////	public class TransientAttribute : Attribute {
////	}

////	public class ConsensusObject {
////		public int ID { get; set; }

////		[Transient]
////		public int ReferenceCount { get; set; }

////	}


////	// 1. Hash the MutatedObjectsInitialState and add them to MutatedObjectProof trees
////	// 2. Verify MutatedObjectProofs recover the leaf nodes of MutatedContainerProof
////	// 3. Verify MutatedContainerProof recovers the start root
////	// At this point the verifier has verified that MutatedObjectsInitialState is correct
////	// 4. Apply all the operation mutations in sequence, fetch objects from MutatedObjectsInitialState as needed
////	// 5. Update MutatedObjectsInitialState with new state of object
////	// 6. For each ClassType, calculate the new merkle-root and store it in the corresponding MutatedContainerProof leaf
////	// 7. Calculate the new root of MutatedContainerProof and this yields the new ObjectSpace Root
////	public class ObjectSpaceMutationProof {

////		// State of object space before transaction
////		public byte[] StartRoot { get; set; }

////		public PartialMerkleTree MutatedContainerProof { get; set; }

////		public IDictionary<int, PartialMerkleTree> MutatedObjectProofs { get; set; }


////		// Key = (ClassType, Instance#), Value = Raw Bytes 
////		public MultiKeyDictionary<int, int, byte[]> MutatedObjectsInitialState { get; set; }

////		public List<byte[]> OperationMutations

////	}


////	public class Proof<TObj> where TObj : ConsensusObject {
////		public TObj Object { get; set; }
////	}


////	public class ConsensusObjectFactory {


////		public TObj New<TObj>() where TObj : ConsensusObject {
////			throw new NotImplementedException();
////		}

////		public bool TryGet<TObj>(int index, out TObj obj) where TObj : ConsensusObject {
////			throw new NotImplementedException();
////		}


////	}


////}



