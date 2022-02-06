using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC; 

public class MuSigSession {
	public byte[] SessionId { get; internal set; }
	public byte[] MessageDigest { get; internal set; }
	public byte[] CombinedPublicKey { get; internal set; }
	public byte[] Nonce { get; internal set; }
	public byte[] Ell { get; internal set; }
	public byte[] Commitment { get; internal set; }
	public bool NonceParity { get; internal set; }
	public bool PublicKeyParity { get; internal set; }
	public bool OwnKeyParity { get; internal set; }
	public bool CombinedNonceParity { get; internal set; }
	public int Idx { get; internal set; }
	public BigInteger SecretKey { get; internal set; }
	public BigInteger SecretNonce { get; internal set; }
	public BigInteger PartialSignature { get; internal set; }
}
