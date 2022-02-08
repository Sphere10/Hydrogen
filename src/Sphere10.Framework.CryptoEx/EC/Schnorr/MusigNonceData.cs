using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Sphere10.Framework.CryptoEx.EC; 

public class MusigNonceData {
	public byte[] PrivateNonce { get; internal set; }
	public byte[] PublicNonce { get; internal set; }
}

internal class MusigPrivateNonce {
	internal byte[] K1 { get; set; }
	internal byte[] K2 { get; set; }
	
	internal byte[] GetFullNonce() {
		return Arrays.Concatenate(K1, K2);
	}
}


internal class MusigPublicNonce {
	internal byte[] R1 { get; set; }
	internal byte[] R2 { get; set; }

	internal byte[] GetFullNonce() {
		return Arrays.Concatenate(R1, R2);
	}
}


public class MusigSessionNonce {
	internal byte[] AggregatedNonce { get; set; }
	internal byte[] FinalNonce { get; set; }
	internal BigInteger NonceCoefficient { get; set; }
	internal bool FinalNonceParity { get; set; }
}
