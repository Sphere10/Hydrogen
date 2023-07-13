// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Runtime.Serialization;

namespace Hydrogen.Application;

public enum ProductDistribution {
	[EnumMember(Value = "Alpha")] Alpha,

	[EnumMember(Value = "Beta")] Beta,

	[EnumMember(Value = "RC")] ReleaseCandidate,

	[EnumMember(Value = "Internal")] InternalRelease
}
