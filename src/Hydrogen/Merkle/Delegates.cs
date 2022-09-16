
using System;
using System.Collections.Generic;

namespace Hydrogen;

public delegate ReadOnlySpan<byte> MerkleTreeLeafGetter(int index);
public delegate int MerkleTreeLeafCounter();
public delegate IEnumerable<MerkleSubRoot> MerkleTreeSubRootsGetter();