// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace Hydrogen.CryptoEx.EC;

public class CustomDsaEncoding : StandardDsaEncoding {
	private const string EmptySignature = "Empty signature";
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
	public static bool IsValidSignatureEncoding(byte[] sig) {
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
		if (lenR > 1 && (sig[4 + additionalOffset] == 0x00) && (sig[5 + additionalOffset] & 0x80) == 0) {
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
		if (n == null) {
			throw new ArgumentNullException(nameof(n));
		}
		if (r == null) {
			throw new ArgumentNullException(nameof(r));
		}
		if (s == null) {
			throw new ArgumentNullException(nameof(s));
		}
		return new DerSequence(
			EncodeValue(n, r),
			EncodeValue(n, EnforceLowS(n, s))
		).GetEncoded(Asn1Encodable.Der);
	}

	public override BigInteger[] Decode(BigInteger n, byte[] encoding) {
		if (n == null) {
			throw new ArgumentNullException(nameof(n));
		}
		if (encoding == null) {
			throw new ArgumentNullException(nameof(encoding));
		}
		if (encoding is { Length: 0 }) {
			throw new ArgumentException(EmptySignature, nameof(encoding));
		}
		if (!IsValidSignatureEncoding(encoding)) {
			throw new ArgumentException(InvalidDerEncoding, nameof(encoding));
		}
		var seq = Asn1Object.FromByteArray(encoding) as Asn1Sequence;
		if (seq is { Count: 2 }) {
			var r = DecodeValue(n, seq, 0);
			var s = DecodeValue(n, seq, 1);

			return new[] { r, s };
		}
		throw new ArgumentException(MalformedSignature, nameof(encoding));
	}
}
