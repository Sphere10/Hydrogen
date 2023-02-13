using System.Runtime.Serialization;
using Tools;
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

	[EnumMember(Value = "pqc-wams")]
	PQC_WAMS,

	[EnumMember(Value = "pqc-wams#")]
	PQC_WAMSSharp,
}