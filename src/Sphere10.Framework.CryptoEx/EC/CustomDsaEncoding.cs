using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC;

public class CustomDsaEncoding : StandardDsaEncoding {
	private bool ForceLowS { get; }

	private static bool IsLowS(BigInteger order, BigInteger s) {
		return s.CompareTo(order.ShiftRight(1)) <= 0;
	}

	/// <summary>
	/// Enforce LowS on the "S" signature
	/// </summary>
	private static BigInteger MakeSCanonical(BigInteger order, BigInteger s) {
		return IsLowS(order, s) ? s : s.Negate().Mod(order);
	}
	public CustomDsaEncoding(bool forceLowS = true) {
		ForceLowS = forceLowS;
	}

	public override byte[] Encode(BigInteger n, BigInteger r, BigInteger s) {
		return new DerSequence(
			EncodeValue(n, r),
			EncodeValue(n, ForceLowS ? MakeSCanonical(n, s) : s)
		).GetEncoded(Asn1Encodable.Der);
	}
}
