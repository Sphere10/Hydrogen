using Org.BouncyCastle.Math.EC;

namespace Sphere10.Framework.CryptoEx.EC; 

public class MuSigData {
	public byte[] CombinedPublicKey { get; internal set; }
	public byte[] MessageDigest { get; internal set; }
	public byte[] CombinedSignature { get; internal set; }
}

public class PublicKeyAggregationData {
	public ECPoint CombinedPoint { get; internal set; }
	public bool PublicKeyParity { get; internal set; }
}
