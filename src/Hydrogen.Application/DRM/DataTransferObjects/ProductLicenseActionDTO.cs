using CommandLine.Core;
using System.Runtime.Serialization;

namespace Hydrogen.Application;

public enum ProductLicenseActionDTO : byte {
	[EnumMember(Value = "enable")]
	Enable = 1,

	[EnumMember(Value = "downgrade")]
	Downgrade,

	[EnumMember(Value = "disable")]
	Disable
}