using Org.BouncyCastle.Math.EC;

namespace Sphere10.Framework.CryptoEx.EC;

public class MuSigData {
	public byte[] AggregatedPublicKey { get; internal set; }
	public byte[] AggregatedSignature { get; internal set; }
}


public class AggregatedPublicKeyData {
	public ECPoint CombinedPoint { get; internal set; }
	public bool PublicKeyParity { get; internal set; }
}
