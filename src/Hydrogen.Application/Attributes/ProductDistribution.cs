using System.Runtime.Serialization;

namespace Hydrogen.Application;

public enum ProductDistribution {
	[EnumMember(Value = "Alpha")]
	Alpha,

	[EnumMember(Value = "Beta")]
	Beta,

	[EnumMember(Value = "RC")]
	ReleaseCandidate,

	[EnumMember(Value = "Internal")]
	InternalRelease
}
