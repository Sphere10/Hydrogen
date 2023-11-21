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

namespace Hydrogen.Application;

[Obfuscation(Exclude = true)]
public class ProductInformation : SettingsObject {
	public string CompanyName { get; set; }
	public string CompanyNumber { get; set; }
	public string ProductName { get; set; }
	public string ProductDescription { get; set; }
	public Guid ProductCode { get; set; }
	public ApplicationVersion ProductVersion { get; set; }
	public string CopyrightNotice { get; set; }
	public string CompanyUrl { get; set; }
	public string ProductUrl { get; set; }
	public string AuthorName { get; set; }
	public string AuthorEmail { get; set; }
	public string ProductPurchaseUrl { get; set; }
	public ProductLicenseActivationDTO DefaultProductLicense { get; set; }
	public string ProductDrmApiUrl { get; set; }
	public ICollection<Tuple<HelpType, string>> HelpResources;
}
