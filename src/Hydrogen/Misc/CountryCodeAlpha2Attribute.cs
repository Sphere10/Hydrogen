using System;

namespace Hydrogen;

public class CountryCodeAlpha2Attribute : Attribute {
	public CountryCodeAlpha2Attribute(string alpha2) {
		Code = alpha2;
	}

	public string Code { get; init; }
}
