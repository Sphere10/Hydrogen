using System;
using System.IO;

namespace Hydrogen.Application {
	public class ApplicationTokenResolver : ITokenResolver {

		private IFuture<StandardProductInformationServices> _productInfoServices = Tools.Values.Future.LazyLoad(() => new StandardProductInformationServices());
		
		public bool TryResolve(string token, out string value) {
			value = token.ToUpperInvariant() switch {
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
			return value != null;
		}
	}
}
