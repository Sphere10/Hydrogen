using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen.Application;
public class ProductLicenseLimitException : SoftwareException {
	public ProductLicenseLimitException(string message) : base(message) {
	}

}

