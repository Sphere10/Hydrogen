// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Application;

public class AssemblyAttributesProductInformationProvider : IProductInformationProvider {
	private ProductInformation _productInformation;

	public AssemblyAttributesProductInformationProvider() {
		_productInformation = null;
	}

	public ProductInformation ProductInformation {
		get {
			if (_productInformation != null)
				return _productInformation;
			lock (this) {
				_productInformation ??= GetProductInformation();
			}
			return _productInformation;
		}
	}

	protected ProductInformation GetProductInformation() {

		var version = ApplicationVersion.Parse(HydrogenAssemblyAttributesHelper.GetAssemblyVersion());
		version.Distribution = HydrogenAssemblyAttributesHelper.GetAssemblyProductDistribution() ?? ProductDistribution.ReleaseCandidate;

		return new ProductInformation {
			CompanyName = HydrogenAssemblyAttributesHelper.GetAssemblyCompany() ?? string.Empty,
			CompanyNumber = HydrogenAssemblyAttributesHelper.GetAssemblyCompanyNumber() ?? string.Empty,
			CompanyUrl = HydrogenAssemblyAttributesHelper.GetAssemblyCompanyLink() ?? string.Empty,
			CopyrightNotice = HydrogenAssemblyAttributesHelper.GetAssemblyCopyright() ?? string.Empty,
			DefaultProductLicense = HydrogenAssemblyAttributesHelper.GetAssemblyDefaultProductLicenseActivation(),
			ProductDrmApiUrl = HydrogenAssemblyAttributesHelper.GetProductDrmApi() ?? string.Empty,
			ProductCode = HydrogenAssemblyAttributesHelper.GetAssemblyProductCode() ?? Guid.Empty,
			ProductDescription = HydrogenAssemblyAttributesHelper.GetAssemblyDescription() ?? string.Empty,
			ProductName = HydrogenAssemblyAttributesHelper.GetAssemblyProduct() ?? string.Empty,
			ProductPurchaseUrl = HydrogenAssemblyAttributesHelper.GetAssemblyProductPurchaseLink() ?? string.Empty,
			ProductUrl = HydrogenAssemblyAttributesHelper.GetAssemblyProductLink() ?? string.Empty,
			AuthorName = HydrogenAssemblyAttributesHelper.GetAssemblyAuthorName() ?? string.Empty,
			AuthorEmail = HydrogenAssemblyAttributesHelper.GetAssemblyAuthorEmail() ?? string.Empty,
			ProductVersion = version,
			HelpResources = HydrogenAssemblyAttributesHelper.GetAssemblyProductHelpResources()
		};
	}

}
