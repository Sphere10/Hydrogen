//-----------------------------------------------------------------------
// <copyright file="StandardProductInformationProvider.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Application {

	public class StandardProductInformationServices : IProductInformationServices {
		ProductInformation _productInformation;

		public StandardProductInformationServices() {
			_productInformation = null;	
		}
		
		public ProductInformation ProductInformation {
			get {
				if (_productInformation == null) {
					lock (this) {
						if (_productInformation == null) {
							_productInformation = GetProductInformation();
						}
					}
				}
				return _productInformation;
			}
		}

		protected ProductInformation GetProductInformation() {
			var manager = new AssemblyAttributesManager();

			string longVersion = manager.GetAssemblyVersion();
			string shortVersion = longVersion;
			string[] versions = longVersion.Split('.');
			if (versions.Length >= 2) {
				shortVersion = string.Format("{0}.{1}", versions[0], versions[1]);
			}

			string defaultKey = Tools.Exceptions.TryOrDefault( () => manager.GetAssemblyDefaultProductKey(), string.Empty);


			return new ProductInformation {
				CompanyName = Tools.Exceptions.TryOrDefault(() => manager.GetAssemblyCompany(), string.Empty),
                CompanyNumber = Tools.Exceptions.TryOrDefault(() =>  manager.GetAssemblyCompanyNumber(), string.Empty),
				CompanyUrl = Tools.Exceptions.TryOrDefault(() =>  manager.GetAssemblyCompanyLink(), string.Empty),
				CopyrightNotice = Tools.Exceptions.TryOrDefault(() =>  manager.GetAssemblyCopyright(), string.Empty),
				DefaultProductKey = Tools.Exceptions.TryOrDefault(() => manager.GetAssemblyDefaultProductKey(), string.Empty),
				ProductCode = Tools.Exceptions.TryOrDefault(() =>  manager.GetAssemblyProductCode(), Guid.Empty),
				ProductDescription = Tools.Exceptions.TryOrDefault(() => manager.GetAssemblyDescription(), string.Empty),
				ProductLongVersion = longVersion,
				ProductName = Tools.Exceptions.TryOrDefault(() =>  manager.GetAssemblyProduct(), string.Empty),
				ProductPurchaseUrl = Tools.Exceptions.TryOrDefault(() =>  manager.GetAssemblyProductPurchaseLink(), string.Empty),
				ProductUrl = Tools.Exceptions.TryOrDefault(() =>  manager.GetAssemblyProductLink(), string.Empty),
				AuthorName = Tools.Exceptions.TryOrDefault(() =>  manager.GetAssemblyAuthorName(), string.Empty),
				AuthorEmail = Tools.Exceptions.TryOrDefault(() =>  manager.GetAssemblyAuthorEmail(), string.Empty),
				ProductVersion = shortVersion,
                HelpResources = Tools.Exceptions.TryOrDefault(() =>  manager.GetAssemblyProductHelpResources(), null)
            };
		}

	}
}
