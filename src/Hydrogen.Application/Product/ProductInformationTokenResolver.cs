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
			"PRODUCTVERSION" => info.ProductVersion,
			"PRODUCTLONGVERSION" => info.ProductLongVersion,
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