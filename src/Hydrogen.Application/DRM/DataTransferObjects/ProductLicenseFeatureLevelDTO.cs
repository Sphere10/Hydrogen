using CommandLine.Core;
using System.Runtime.Serialization;

namespace Hydrogen.Application;

public enum ProductLicenseFeatureLevelDTO : byte {
	[EnumMember(Value = "none")]
	None = 1,

	[EnumMember(Value = "free")]
	Free,

	[EnumMember(Value = "tier1")]
	Tier1,

	[EnumMember(Value = "tier2")]
	Tier2,

	[EnumMember(Value = "tier3")]
	Tier3,

	[EnumMember(Value = "full")]
	Full,
}