using System;
using System.IO;

namespace Sphere10.Framework.Application {
	public class ApplicationTokenResolver : ITokenResolver {

		private IFuture<StandardProductInformationServices> _productInfoServices = Tools.Values.LazyLoad(() => new StandardProductInformationServices());

		public string TryResolve(string token) {
			return token.ToUpperInvariant() switch {
				"USERDATADIR" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)), 
				"SYSTEMDATADIR" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)),
				"COMPANYNAME" => _productInfoServices.Value.ProductInformation.CompanyName,
				"COMPANYNUMBER" => _productInfoServices.Value.ProductInformation.CompanyNumber,
				"PRODUCTNAME" => _productInfoServices.Value.ProductInformation.ProductName,
				"PRODUCTDESCRIPTION" => _productInfoServices.Value.ProductInformation.ProductDescription,
				"PRODUCTCODE" => _productInfoServices.Value.ProductInformation.ProductCode.ToStrictAlphaString(),
				"PRODUCTVERSION" => _productInfoServices.Value.ProductInformation.ProductVersion,
				"PRODUCTLONGVERSION" => _productInfoServices.Value.ProductInformation.ProductLongVersion,
				"COPYRIGHTNOTICE" => _productInfoServices.Value.ProductInformation.CopyrightNotice,
				"COMPANYURL" => _productInfoServices.Value.ProductInformation.CompanyUrl,
				"PRODUCTURL" => _productInfoServices.Value.ProductInformation.ProductUrl,
				"PRODUCTPURCHASEURL" => _productInfoServices.Value.ProductInformation.ProductPurchaseUrl,
				_ => null
			};
		}
	}
}
