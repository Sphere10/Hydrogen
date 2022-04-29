using Org.BouncyCastle.Math;

namespace Hydrogen.CryptoEx.EC;

public class MuSigSessionCache {
	public bool FinalNonceParity { get; internal set; }
	public bool PublicKeyParity { get; internal set; }
	public byte[] FinalNonce { get; internal set; }
	public BigInteger Challenge { get; internal set; }
	public BigInteger NonceCoefficient { get; internal set; }
}
