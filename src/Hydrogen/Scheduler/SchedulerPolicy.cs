//-----------------------------------------------------------------------
// <copyright file="SchedulerPolicy.cs" company="Sphere 10 Software">
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

namespace Hydrogen {

	[Flags]
	public enum SchedulerPolicy {
		ForceSyncronous		= 1 << 0,
		DontThrow			= 1 << 1,
		RemoveJobOnError	= 1 << 2,
		DisposeWhenFinished	= 1 << 3,
	}
}
