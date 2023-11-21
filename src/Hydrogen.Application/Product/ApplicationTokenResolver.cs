// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen.Application;

public class ApplicationTokenResolver : ITokenResolver {

	public ApplicationTokenResolver(IProductInformationProvider productInformationProvider) {
		ProductInformationProvider = productInformationProvider;
	}

	protected IProductInformationProvider ProductInformationProvider { get; }

	public bool TryResolve(string token, out object value) {
		value = token.ToUpperInvariant() switch {
			"USERDATADIR" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)),
			"SYSTEMDATADIR" => Path.Combine(Environment.GetFolderPath(Environment.OSVersion.Platform != PlatformID.Unix
				? Environment.SpecialFolder.CommonApplicationData
				: Environment.SpecialFolder.LocalApplicationData)), // CommonApplicationData is readonly on Linux
			"COMPANYNAME" => ProductInformationProvider.ProductInformation.CompanyName,
			"COMPANYNUMBER" => ProductInformationProvider.ProductInformation.CompanyNumber,
			"PRODUCTNAME" => ProductInformationProvider.ProductInformation.ProductName,
			"PRODUCTDESCRIPTION" => ProductInformationProvider.ProductInformation.ProductDescription,
			"PRODUCTCODE" => ProductInformationProvider.ProductInformation.ProductCode.ToStrictAlphaString(),
			"PRODUCTVERSION" => ProductInformationProvider.ProductInformation.ProductVersion.ToString("S", null),
			"PRODUCTLONGVERSION" => ProductInformationProvider.ProductInformation.ProductVersion.ToString(),
			"COPYRIGHTNOTICE" => ProductInformationProvider.ProductInformation.CopyrightNotice,
			"COMPANYURL" => ProductInformationProvider.ProductInformation.CompanyUrl,
			"PRODUCTURL" => ProductInformationProvider.ProductInformation.ProductUrl,
			"PRODUCTPURCHASEURL" => ProductInformationProvider.ProductInformation.ProductPurchaseUrl,
			"CURRENTYEAR" => DateTime.Now.Year.ToString(),
			"STARTPATH" => Path.GetDirectoryName(Tools.Runtime.GetExecutablePath()),
			_ => null
		};
		return value != null;
	}
}
