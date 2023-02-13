//-----------------------------------------------------------------------
// <copyright file="ProductInformation.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;


namespace Hydrogen.Application;

[Obfuscation(Exclude = true)]
public class ProductInformation {
	public string CompanyName { get; set; }
	public string CompanyNumber { get; set; }
	public string ProductName { get; set; }
	public string ProductDescription { get; set; }
	public Guid ProductCode { get; set; }
	public string ProductVersion { get; set; }
	public string ProductLongVersion { get; set; }
	public string CopyrightNotice { get; set; }
	public string CompanyUrl { get; set; }
	public string ProductUrl { get; set; }
	public string AuthorName { get; set; }
	public string AuthorEmail { get; set; }
	public string ProductPurchaseUrl { get; set; }
	public ProductLicenseActivationDTO DefaultProductLicense { get; set; }
	public string ProductDrmApiUrl { get; set; }

	public ICollection<Tuple<HelpType, string>> HelpResources;

	public byte GetProductMajorVersion() {
		return CalculateMajorVersion(ProductVersion);
	}

	public static byte CalculateMajorVersion(string productVersion) {
		var splits = productVersion.Split(new char[] { '.' });
		byte majorVersion = 0;
		if (splits.Length > 0) {
			if (!byte.TryParse(splits[0], out majorVersion)) {
				throw new SoftwareException("Unable to determine major version of product version {0}", productVersion);
			}
		}
		return majorVersion;
	}

}


