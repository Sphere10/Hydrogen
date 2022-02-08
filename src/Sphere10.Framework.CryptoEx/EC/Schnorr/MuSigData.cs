using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC; 

public class MuSigData {
	public byte[] CombinedPublicKey { get; internal set; }
	public byte[] MessageDigest { get; internal set; }
	public byte[] Signature { get; internal set; }
}


public class PartialMuSig {
	public BigInteger Signature { get; internal set; }
}
