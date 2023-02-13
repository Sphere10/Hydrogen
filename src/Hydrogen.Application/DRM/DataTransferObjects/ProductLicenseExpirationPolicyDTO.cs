using CommandLine.Core;
using System.Runtime.Serialization;

namespace Hydrogen.Application;

public enum ProductLicenseExpirationPolicyDTO : byte {
	[EnumMember(Value = "none")]
	None = 1,

	[EnumMember(Value = "downgrade")]
	Downgrade,

	[EnumMember(Value = "disable")]
	Disable
}