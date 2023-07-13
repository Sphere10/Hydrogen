// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Hydrogen.Application;

/// <summary>
/// Describes a product license suitable for transfer.
/// </summary>
// NOTE: if updating this ensure that ProductLicenseDTOSerializer is also updated
[Obfuscation(Exclude = true)]
public class ProductLicenseDTO {

	[JsonProperty("name")] public string Name { get; set; }

	[JsonProperty("productKey")] public string ProductKey { get; set; }

	[JsonProperty("productCode")] public Guid ProductCode { get; set; }

	[JsonProperty("featureLevel")]
	[JsonConverter(typeof(StringEnumConverter))]
	public ProductLicenseFeatureLevelDTO FeatureLevel { get; set; }

	[JsonProperty("expirationPolicy")]
	[JsonConverter(typeof(StringEnumConverter))]
	public ProductLicenseExpirationPolicyDTO ExpirationPolicy { get; set; }

	[JsonProperty("majorVersionApplicable", NullValueHandling = NullValueHandling.Ignore)]
	public short? MajorVersionApplicable { get; set; }

	[JsonProperty("expirationDate", NullValueHandling = NullValueHandling.Ignore)]
	public DateTime? ExpirationDate { get; set; }

	[JsonProperty("expirationDays", NullValueHandling = NullValueHandling.Ignore)]
	public int? ExpirationDays { get; set; }

	[JsonProperty("expirationLoads", NullValueHandling = NullValueHandling.Ignore)]
	public int? ExpirationLoads { get; set; }

	[JsonProperty("maxConcurrentInstances", NullValueHandling = NullValueHandling.Ignore)]
	public int? MaxConcurrentInstances { get; set; }

	[JsonProperty("maxSeats", NullValueHandling = NullValueHandling.Ignore)]
	public int? MaxSeats { get; set; }

	[JsonProperty("limitFeatureA", NullValueHandling = NullValueHandling.Ignore)]
	public int? LimitFeatureA { get; set; }

	[JsonProperty("limitFeatureB", NullValueHandling = NullValueHandling.Ignore)]
	public int? LimitFeatureB { get; set; }

	[JsonProperty("limitFeatureC", NullValueHandling = NullValueHandling.Ignore)]
	public int? LimitFeatureC { get; set; }

	[JsonProperty("limitFeatureD", NullValueHandling = NullValueHandling.Ignore)]
	public int? LimitFeatureD { get; set; }

}
