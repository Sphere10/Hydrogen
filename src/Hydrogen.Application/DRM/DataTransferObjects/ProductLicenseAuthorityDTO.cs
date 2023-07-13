// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hydrogen.Application;

public class ProductLicenseAuthorityDTO : IEquatable<ProductLicenseAuthorityDTO> {

	[JsonProperty("name")] public string Name { get; set; }

	[JsonProperty("dss")]
	[JsonConverter(typeof(StringEnumConverter))]
	public DSS LicenseDSS { get; set; }

	[JsonProperty("publicKey")] public byte[] LicensePublicKey { get; set; }

	public bool Equals(ProductLicenseAuthorityDTO other) {
		if (ReferenceEquals(null, other))
			return false;
		if (ReferenceEquals(this, other))
			return true;
		return Name == other.Name && LicenseDSS == other.LicenseDSS && ByteArrayEqualityComparer.Equals(LicensePublicKey, other.LicensePublicKey);
	}
	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj))
			return false;
		if (ReferenceEquals(this, obj))
			return true;
		if (obj.GetType() != this.GetType())
			return false;
		return Equals((ProductLicenseAuthorityDTO)obj);
	}
	public override int GetHashCode() {
		return HashCode.Combine(Name, (int)LicenseDSS, LicensePublicKey);
	}
}
