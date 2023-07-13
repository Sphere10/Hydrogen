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

		var releaseType = HydrogenAssemblyAttributesHelper.GetAssemblyProductDistribution() ?? ProductDistribution.ReleaseCandidate;
		var longVersion = HydrogenAssemblyAttributesHelper.GetAssemblyVersion();
		var shortVersion = longVersion;
		var versions = longVersion.Split('.');
		if (versions.Length >= 2) {
			shortVersion = $"{versions[0]}.{versions[1]}";
		}

		var releaseTypeCode = releaseType switch {
			ProductDistribution.ReleaseCandidate => string.Empty,
			_ => $" ({Tools.Enums.GetSerializableOrientedName(releaseType)})"
		};

		return new ProductInformation {
			CompanyName = HydrogenAssemblyAttributesHelper.GetAssemblyCompany() ?? string.Empty,
			CompanyNumber = HydrogenAssemblyAttributesHelper.GetAssemblyCompanyNumber() ?? string.Empty,
			CompanyUrl = HydrogenAssemblyAttributesHelper.GetAssemblyCompanyLink() ?? string.Empty,
			CopyrightNotice = HydrogenAssemblyAttributesHelper.GetAssemblyCopyright() ?? string.Empty,
			DefaultProductLicense = HydrogenAssemblyAttributesHelper.GetAssemblyDefaultProductLicenseActivation(),
			ProductDrmApiUrl = HydrogenAssemblyAttributesHelper.GetProductDrmApi() ?? string.Empty,
			ProductCode = HydrogenAssemblyAttributesHelper.GetAssemblyProductCode() ?? Guid.Empty,
			ProductDescription = HydrogenAssemblyAttributesHelper.GetAssemblyDescription() ?? string.Empty,
			ProductLongVersion = longVersion,
			ProductName = HydrogenAssemblyAttributesHelper.GetAssemblyProduct() ?? string.Empty,
			ProductPurchaseUrl = HydrogenAssemblyAttributesHelper.GetAssemblyProductPurchaseLink() ?? string.Empty,
			ProductUrl = HydrogenAssemblyAttributesHelper.GetAssemblyProductLink() ?? string.Empty,
			AuthorName = HydrogenAssemblyAttributesHelper.GetAssemblyAuthorName() ?? string.Empty,
			AuthorEmail = HydrogenAssemblyAttributesHelper.GetAssemblyAuthorEmail() ?? string.Empty,
			ProductVersion = ($"{shortVersion}{releaseTypeCode}").Trim(),
			HelpResources = HydrogenAssemblyAttributesHelper.GetAssemblyProductHelpResources()
		};
	}

}
