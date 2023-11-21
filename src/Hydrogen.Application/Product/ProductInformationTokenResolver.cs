// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Linq;

namespace Hydrogen.Application;

public class ProductInformationTokenResolver : ITokenResolver {

	public ProductInformationTokenResolver(IProductInformationProvider productInformationProvider) {
		ProductInformationProvider = productInformationProvider;
	}

	protected IProductInformationProvider ProductInformationProvider { get; }

	public bool TryResolve(string token, out object value) {
		var info = ProductInformationProvider.ProductInformation;
		value = token.ToUpperInvariant() switch {
			"COMPANYNAME" => info.CompanyName,
			"COMPANYNUMBER" => info.CompanyNumber,
			"PRODUCTNAME" => info.ProductName,
			"PRODUCTDESCRIPTION" => info.ProductDescription,
			"PRODUCTCODE" => info.ProductCode.ToString().SkipWhile(c => c == '{').TakeWhile(c => c != '}').ToString(),
			"PRODUCTVERSION" => info.ProductVersion.ToString("S", null),
			"PRODUCTLONGVERSION" => info.ProductVersion.ToString("D", null),
			"COPYRIGHTNOTICE" => info.CopyrightNotice,
			"COMPANYURL" => info.CompanyUrl,
			"PRODUCTURL" => info.ProductUrl,
			"PRODUCTPURCHASEURL" => info.ProductPurchaseUrl,
			"PRODUCTDRMAPIURL" => info.ProductDrmApiUrl,
			"DEFAULTPRODUCTKEY" => info.DefaultProductLicense?.License?.Item?.ProductKey,
			_ => null
		};
		return value != null;
	}
}
