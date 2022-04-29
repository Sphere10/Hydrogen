//-----------------------------------------------------------------------
// <copyright file="DBKeyType.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Data {

	[Flags]
	public enum DBKeyType {
		None							= 0,
		AutoCalculated					= 1 << 0,
		RootIsAutoCalculated			= 1 << 1,
		Sequenced						= 1 << 2,
		RootIsSequenced					= 1 << 3,
		ManuallyAssignedSingleInt		= 1 << 4,
		RootIsManuallyAssignedSingleInt	= 1 << 5,
		Artificial						= 1 << 6,
		RootIsArtificial				= 1 << 7
	}
}
