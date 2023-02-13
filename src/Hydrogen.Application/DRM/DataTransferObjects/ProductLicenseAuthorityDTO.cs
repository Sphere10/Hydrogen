using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hydrogen.Application;

public class ProductLicenseAuthorityDTO : IEquatable<ProductLicenseAuthorityDTO> {

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("dss")]
	[JsonConverter(typeof(StringEnumConverter))]
	public DSS LicenseDSS { get; set; }

	[JsonProperty("publicKey")]
	public byte[] LicensePublicKey { get; set; }

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