using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC;

public class SignerMuSigSession {
	public bool InternalKeyParity { get; internal set; }
	public BigInteger SecretKey { get; internal set; }
	public BigInteger KeyCoefficient { get; internal set; }
	public byte[] PublicNonce { get; internal set; }
	public byte[] PrivateNonce { get; internal set; }
}
