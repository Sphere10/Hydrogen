//-----------------------------------------------------------------------
// <copyright file="ProductRights.cs" company="Sphere 10 Software">
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
using System.Reflection;


namespace Hydrogen.Application {
	[Obfuscation(Exclude = true)]
    public class ProductRights {
		private static ProductRights _none;

		static ProductRights() {
			_none = new ProductRights();
		}

		public ProductRights() {
			ExpiresAfterDate = true;
			ExpirationDateUTC = DateTime.UtcNow;
			HasFiniteUses = true;
			UsesRemaining = 0;
			TotalUsesAllowed = 0;
			HasFiniteDays = true;
			DaysRemaining = 0;
			TotalDaysAllowed = 0;
			HasFiniteInstances = true;
			InstancesRemaining = 0;
			TotalInstancesAllowed = 0;
			FeatureRights = ProductFeatureRights.None;
		}

		public static ProductRights None { get { return _none; } }

		public bool HasFiniteUses { get; set; }

		public int UsesRemaining { get; set; }

		public int TotalUsesAllowed { get; set; }

		public bool HasFiniteDays { get; set; }

		public int DaysRemaining { get; set; }

		public int TotalDaysAllowed { get; set; }

		public bool ExpiresAfterDate { get; set; }

		public DateTime ExpirationDateUTC { get; set; }

		public bool HasFiniteInstances { get; set; }

		public int InstancesRemaining { get; set; }

		public int TotalInstancesAllowed { get; set; }

		public bool AppliesToVersion { get; set; }

		public int Version { get; set; }

		public ProductFeatureRights FeatureRights;


	}
	    
}
