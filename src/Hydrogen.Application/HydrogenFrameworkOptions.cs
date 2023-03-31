using System;

namespace Hydrogen.Application;

[Flags]
public enum HydrogenFrameworkOptions {
	EnableDrm = 1 << 0,
	BackgroundLicenseVerify = 1 << 1,
	Default = 0,
}
