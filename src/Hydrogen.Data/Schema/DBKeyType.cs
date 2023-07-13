// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Data;

[Flags]
public enum DBKeyType {
	None = 0,
	AutoCalculated = 1 << 0,
	RootIsAutoCalculated = 1 << 1,
	Sequenced = 1 << 2,
	RootIsSequenced = 1 << 3,
	ManuallyAssignedSingleInt = 1 << 4,
	RootIsManuallyAssignedSingleInt = 1 << 5,
	Artificial = 1 << 6,
	RootIsArtificial = 1 << 7
}
