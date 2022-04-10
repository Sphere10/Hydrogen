
using System;
using System.Collections.Generic;

namespace Sphere10.Framework; 

public delegate ReadOnlySpan<byte> MerkleTreeLeafGetter(int index);
public delegate int MerkleTreeLeafCounter();
public delegate IEnumerable<MerkleSubRoot> MerkleTreeSubRootsGetter();