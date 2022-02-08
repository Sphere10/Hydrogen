using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC; 

public class MuSigSession {
	public byte[] SessionId { get; internal set; }
	public byte[] MessageDigest { get; internal set; }
	public byte[] CombinedPublicKey { get; internal set; }
	public byte[] Ell { get; internal set; }
	public bool PublicKeyParity { get; internal set; }
	public bool InternalKeyParity { get; internal set; }
	public bool FinalNonceParity { get; internal set; }
	public int Idx { get; internal set; }
	public BigInteger SecretKey { get; internal set; }
	public BigInteger KeyCoefficient { get; internal set; }
	public BigInteger NonceCoefficient { get; internal set; }
	public byte[] PublicNonce { get; internal set; }
	public byte[] PrivateNonce { get; internal set; }
	public byte[] AggregatedNonce { get; internal set; }
	public byte[] FinalNonce { get; internal set; }
	public BigInteger Challenge { get; internal set; }
	public BigInteger PartialSignature { get; internal set; }
}
