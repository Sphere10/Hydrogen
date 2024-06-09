// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Runtime.Serialization;

namespace Hydrogen;

public enum DSS : byte {

	[EnumMember(Value = "ecdsa-secp256k1")]
	ECDSA_SECP256k1 = 1,

	[EnumMember(Value = "ecdsa-secp384r1")]
	ECDSA_SECP384R1,

	[EnumMember(Value = "ecdsa-secp521r1")]
	ECDSA_SECP521R1,

	[EnumMember(Value = "ecdsa-sect283k1")]
	ECDSA_SECT283K1,

	[EnumMember(Value = "schnorr-secp256k1")]
	SCHNORR_SECP256k1,

	[EnumMember(Value = "schnorr-secp384r1")]
	SCHNORR_SECP384R1,

	[EnumMember(Value = "schnorr-secp521r1")]
	SCHNORR_SECP521R1,

	[EnumMember(Value = "schnorr-sect283k1")]
	SCHNORR_SECT283K1,

	[EnumMember(Value = "pqc-wams")] PQC_WAMS,

	[EnumMember(Value = "pqc-wams#")] PQC_WAMSSharp,
}
