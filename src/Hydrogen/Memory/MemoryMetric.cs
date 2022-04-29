//-----------------------------------------------------------------------
// <copyright file="MemoryMetric.cs" company="Sphere 10 Software">
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

using System.ComponentModel;

namespace Sphere10.Framework {
	public enum MemoryMetric {
        [Description("bits")]
        Bit = 0,
		
        [Description("kbits")]
        Kilobit,

		[Description("mbits")]
        Megabit,

		[Description("gbits")]
        Gigabit,

		[Description("tbits")]
        Terrabit,

		[Description("pbits")]
        Petabit,

		[Description("b")]
        Byte,

		[Description("kb")]
        Kilobyte,

		[Description("mb")]
        Megabyte,

		[Description("gb")]
        Gigabyte,

		[Description("tb")]
        Terrabyte,

        [Description("pb")] 
        PetaByte
    }
}
