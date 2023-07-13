// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Runtime.Serialization;

namespace Hydrogen.Application;

public enum ProductLicenseFeatureLevelDTO : byte {
	[EnumMember(Value = "none")] None = 1,

	[EnumMember(Value = "free")] Free,

	[EnumMember(Value = "tier1")] Tier1,

	[EnumMember(Value = "tier2")] Tier2,

	[EnumMember(Value = "tier3")] Tier3,

	[EnumMember(Value = "full")] Full,
}
