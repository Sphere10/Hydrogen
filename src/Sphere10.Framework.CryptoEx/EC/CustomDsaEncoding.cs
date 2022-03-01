using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC;

public class CustomDsaEncoding : StandardDsaEncoding {
	private const string MalformedSignature = "Malformed signature";
	private const string InvalidDerEncoding = "Signature does not conform to strict der encoding";
	private bool ForceLowS { get; }

	private static bool IsLowS(BigInteger order, BigInteger s) {
		return s.CompareTo(order.ShiftRight(1)) <= 0;
	}

	/// <summary>
	/// Strict DER signatures (with adaptations to account for other curves)
	/// https://github.com/bitcoin/bips/blob/master/bip-0066.mediawiki
	/// </summary>
	/// <param name="sig"></param>
	/// <returns></returns>
	private static bool IsValidSignatureEncoding(byte[] sig) {
		var sigLength = sig.Length;

		// A signature is of type 0x30 (compound).
		if (sig[0] != 0x30) {
			return false;
		}

		// Make sure the length covers the entire signature.
		var valueAtStartOffset = sig[1];
		var additionalOffset = valueAtStartOffset < 0x80 ? 0 : valueAtStartOffset & 0x7F;

		if (sig[1 + additionalOffset] != sigLength - (2 + additionalOffset)) {
			return false;
		}

		// Extract the length of the R element.
		uint lenR = sig[3 + additionalOffset];

		// Make sure the length of the S element is still inside the signature.
		if (5 + lenR + additionalOffset >= sigLength) {
			return false;
		}

		// Extract the length of the S element.
		uint lenS = sig[5 + additionalOffset + lenR];

		// Verify that the length of the signature matches the sum of the length
		// of the elements.
		if ((lenR + lenS + 6 + additionalOffset) != sigLength) {
			return false;
		}

		// Check whether the R element is an integer.
		if (sig[2 + additionalOffset] != 0x02) {
			return false;
		}

		// Zero-length integers are not allowed for R.
		if (lenR == 0) {
			return false;
		}

		// Negative numbers are not allowed for R.
		if ((sig[4 + additionalOffset] & 0x80) != 0) {
			return false;
		}

		// Null bytes at the start of R are not allowed, unless R would
		// otherwise be interpreted as a negative number.
		if (lenR > 1 && (sig[4] == 0x00) && (sig[5] & 0x80) == 0) {
			return false;
		}

		// Check whether the S element is an integer.
		if (sig[lenR + 4 + additionalOffset] != 0x02) {
			return false;
		}

		// Zero-length integers are not allowed for S.
		if (lenS == 0) {
			return false;
		}

		// Negative numbers are not allowed for S.
		if ((sig[lenR + 6 + additionalOffset] & 0x80) != 0) {
			return false;
		}

		// Null bytes at the start of S are not allowed, unless S would otherwise be
		// interpreted as a negative number.
		if (lenS > 1 && (sig[lenR + 6 + additionalOffset] == 0x00) && (sig[lenR + 7 + additionalOffset] & 0x80) == 0) {
			return false;
		}
		return true;
	}

	/// <summary>
	/// Enforce LowS on the "S" signature
	/// https://github.com/bitcoin/bips/blob/master/bip-0146.mediawiki
	/// https://github.com/bitcoin/bitcoin/pull/6769
	/// </summary>
	private BigInteger EnforceLowS(BigInteger order, BigInteger s) {
		return ForceLowS ? IsLowS(order, s) ? s : order.Subtract(s) : s;
	}
	public CustomDsaEncoding(bool forceLowS = true) {
		ForceLowS = forceLowS;
	}

	public override byte[] Encode(BigInteger n, BigInteger r, BigInteger s) {
		return new DerSequence(
			EncodeValue(n, r),
			EncodeValue(n, EnforceLowS(n, s))
		).GetEncoded(Asn1Encodable.Der);

		// MemoryStream stream = new MemoryStream();
		// Asn1OutputStream der = Asn1OutputStream.Create(stream);
		//
		// Asn1EncodableVector v = new Asn1EncodableVector();
		// v.Add(new DerInteger(r));
		// v.Add(new DerInteger(s));
		// der.WriteObject(new DerSequence(v));

		// var tmp = stream.ToArray();

		// Usually 70-72 bytes.
		// MemoryStream bos = new MemoryStream();
		// DerSequenceGenerator seq = new DerSequenceGenerator(bos);
		// seq.AddObject(new DerInteger(r));
		// //seq.AddObject(new DerInteger(s));
		// seq.AddObject(new DerInteger(EnforceLowS(n, s)));
		// seq.Close();
		// var tmp = bos.ToArray();
	}

	public override BigInteger[] Decode(BigInteger n, byte[] encoding) {
		if (!IsValidSignatureEncoding(encoding)) {
			throw new ArgumentException(InvalidDerEncoding, nameof(encoding));
		}
		;
		var seq = Asn1Object.FromByteArray(encoding) as Asn1Sequence;
		if (seq is { Count: 2 }) {
			var r = DecodeValue(n, seq, 0);
			var s = DecodeValue(n, seq, 1);

			return new[] { r, s };
		}
		throw new ArgumentException(MalformedSignature, nameof(encoding));

		// try
		// {
		// 	Asn1InputStream decoder = new Asn1InputStream(derSig);
		// 	var seq = decoder.ReadObject() as DerSequence;
		// 	if (seq == null || seq.Count != 2)
		// 		throw new FormatException(InvalidDERSignature);
		// 	_R = ((DerInteger)seq[0]).Value;
		// 	_S = ((DerInteger)seq[1]).Value;
		// }
		// catch (Exception ex)
		// {
		// 	throw new FormatException(InvalidDERSignature, ex);
		// }
	}
}
