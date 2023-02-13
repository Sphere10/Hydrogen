//-----------------------------------------------------------------------
// <copyright file="ProductUsageInformation.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Reflection;

namespace Hydrogen.Application {

	public class ProductUsageInformation {

		public DateTime FirstUsedDateBySystemUTC { get; set; }
		public int DaysUsedBySystem { get; set; }
		public int NumberOfUsesBySystem { get; set; }
		public DateTime FirstUsedDateByUserUTC { get; set; }
		public int DaysUsedByUser { get; set; }
		public int NumberOfUsesByUser { get; set; }


	}
}
