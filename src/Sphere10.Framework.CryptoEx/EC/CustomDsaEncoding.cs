using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC;

public class CustomDsaEncoding : StandardDsaEncoding {
	private const string MalformedSignature = "Malformed signature";
	private bool ForceLowS { get; }

	private static bool IsLowS(BigInteger order, BigInteger s) {
		return s.CompareTo(order.ShiftRight(1)) <= 0;
	}

	/// <summary>
	/// Enforce LowS on the "S" signature
	/// </summary>
	private static BigInteger MakeSCanonical(BigInteger order, BigInteger s) {
		return IsLowS(order, s) ? s : order.Subtract(s);
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

	public override BigInteger[] Decode(BigInteger n, byte[] encoding) {
		var seq = Asn1Object.FromByteArray(encoding) as Asn1Sequence;
		if (seq is { Count: 2 }) {
			var r = DecodeValue(n, seq, 0);
			var s = DecodeValue(n, seq, 1);

			return new[] { r, s };
		}
		throw new ArgumentException(MalformedSignature, nameof(encoding));
	}
}
