using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen.Application;
public class ProductLicenseTamperedException : SoftwareException{
	public ProductLicenseTamperedException() : this(null) {
	}

	public ProductLicenseTamperedException(Exception innerException) : base("The license is invalid and/or has been tampered with", innerException) {
	}
}

