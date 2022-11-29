using System;

namespace Hydrogen;

public class CountryCodeAlpha3Attribute : Attribute {
	public CountryCodeAlpha3Attribute(string alpha3) {
		Code = alpha3;
	}

	public string Code { get; init; }
}
