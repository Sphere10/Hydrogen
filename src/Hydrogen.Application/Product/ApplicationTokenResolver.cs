using System;
using System.IO;
using System.Reflection;

namespace Hydrogen.Application;
public class ApplicationTokenResolver : ITokenResolver {

	public ApplicationTokenResolver(IProductInformationProvider productInformationProvider) {
		ProductInformationProvider = productInformationProvider;
	}

	protected IProductInformationProvider ProductInformationProvider { get; }

	public bool TryResolve(string token, out object value) {
		value = token.ToUpperInvariant() switch {
			"USERDATADIR" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)),
			"SYSTEMDATADIR" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)),
			"COMPANYNAME" => ProductInformationProvider.ProductInformation.CompanyName,
			"COMPANYNUMBER" => ProductInformationProvider.ProductInformation.CompanyNumber,
			"PRODUCTNAME" => ProductInformationProvider.ProductInformation.ProductName,
			"PRODUCTDESCRIPTION" => ProductInformationProvider.ProductInformation.ProductDescription,
			"PRODUCTCODE" => ProductInformationProvider.ProductInformation.ProductCode.ToStrictAlphaString(),
			"PRODUCTVERSION" => ProductInformationProvider.ProductInformation.ProductVersion,
			"PRODUCTLONGVERSION" => ProductInformationProvider.ProductInformation.ProductLongVersion,
			"COPYRIGHTNOTICE" => ProductInformationProvider.ProductInformation.CopyrightNotice,
			"COMPANYURL" => ProductInformationProvider.ProductInformation.CompanyUrl,
			"PRODUCTURL" => ProductInformationProvider.ProductInformation.ProductUrl,
			"PRODUCTPURCHASEURL" => ProductInformationProvider.ProductInformation.ProductPurchaseUrl,
			"CURRENTYEAR" => DateTime.Now.Year.ToString(),
			"STARTPATH" => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
			_ => null
		};
		return value != null;
	}
}