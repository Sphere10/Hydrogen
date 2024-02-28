// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hydrogen.Application;

[Obfuscation(Exclude = true)]
public class ProductRights {

	public static ProductRights None { get; } = new() {
		ExpiresAfterDate = true,
		ExpirationDateUTC = DateTime.UtcNow,
		HasFiniteUses = true,
		UsesRemaining = 0,
		TotalUsesAllowed = 0,
		HasFiniteDays = true,
		DaysRemaining = 0,
		TotalDaysAllowed = 0,
		HasFiniteInstances = true,
		InstancesRemaining = 0,
		TotalInstancesAllowed = 0,
		FeatureRights = ProductLicenseFeatureLevelDTO.None,
	};

	public static ProductRights Full { get; } = new() {
		ExpiresAfterDate = false,
		ExpirationDateUTC = DateTime.MaxValue,
		HasFiniteUses = false,
		UsesRemaining = int.MaxValue,
		TotalUsesAllowed = int.MaxValue,
		HasFiniteDays = false,
		DaysRemaining = int.MaxValue,
		TotalDaysAllowed = int.MaxValue,
		HasFiniteInstances = false,
		InstancesRemaining = int.MaxValue,
		TotalInstancesAllowed = int.MaxValue,
		FeatureRights = ProductLicenseFeatureLevelDTO.Full,
	};

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

	public ProductLicenseFeatureLevelDTO FeatureRights;

	public int? LimitFeatureA { get; set; }

	public int? LimitFeatureB { get; set; }

	public int? LimitFeatureC { get; set; }

	public int? LimitFeatureD { get; set; }

	public void Disable() {
		FeatureRights = ProductLicenseFeatureLevelDTO.None;

		if (AppliesToVersion)
			Version = 0;

		if (HasFiniteUses)
			UsesRemaining = 0;

		if (HasFiniteDays)
			DaysRemaining = 0;

		if (ExpiresAfterDate)
			ExpirationDateUTC = DateTime.UtcNow;

		if (HasFiniteInstances)
			InstancesRemaining = 0;
	}

	public void Enable(ProductLicenseFeatureLevelDTO featureLevel) {
		FeatureRights = featureLevel;
		AppliesToVersion = false;
		HasFiniteUses = false;
		HasFiniteDays = false;
		if (ExpiresAfterDate && ExpirationDateUTC <= DateTime.UtcNow)
			ExpiresAfterDate = false;
		HasFiniteInstances = false;
	}

	public override string ToString() 
		=> ToString("Feature A", "Feature B", "Feature C", "Feature D");

	public string ToString(string featureAName, string featureBName = "Feature B", string featureCName = "Feature C", string featureDName = "Feature D") {
		var msgs = new List<string>();

		msgs.Add($"Feature level permitted by license: {FeatureRights}");
		
		if (LimitFeatureA.HasValue) 
			msgs.Add($"{featureAName?.ToCamelCase()} limit: {LimitFeatureA}");

		if (LimitFeatureB.HasValue) 
			msgs.Add($"{featureBName?.ToCamelCase()} limit: {LimitFeatureB}");

		if (LimitFeatureC.HasValue) 
			msgs.Add($"{featureCName?.ToCamelCase()} limit: {LimitFeatureC}");

		if (LimitFeatureD.HasValue) 
			msgs.Add($"{featureDName?.ToCamelCase()} limit: {LimitFeatureD}");

		if (ExpiresAfterDate)
			msgs.Add($"expires on {ExpirationDateUTC:yyyy-MM-dd}");

		if (HasFiniteDays)
			msgs.Add($"has {DaysRemaining} out of {TotalDaysAllowed} days left");

		if (HasFiniteUses)
			msgs.Add($"has {UsesRemaining} out of {TotalUsesAllowed} executions remaining");

		if (HasFiniteInstances)
			msgs.Add($"has {InstancesRemaining} out of {TotalInstancesAllowed} concurrent instances");

		return msgs.ToDelimittedString(", ");

	}
}
