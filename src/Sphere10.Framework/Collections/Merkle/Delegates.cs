
using System;

namespace Sphere10.Framework; 

public delegate ReadOnlySpan<byte> MerkleTreeLeafGetter(int index);
public delegate int MerkleTreeLeafCounter();