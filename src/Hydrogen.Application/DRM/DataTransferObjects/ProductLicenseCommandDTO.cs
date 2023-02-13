using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hydrogen.Application;

public class ProductLicenseCommandDTO {

	[JsonProperty("productKey", NullValueHandling = NullValueHandling.Ignore)]
	public string ProductKey { get; set; }

	[JsonProperty("action", NullValueHandling = NullValueHandling.Ignore)]
	[JsonConverter(typeof(StringEnumConverter))]
	public ProductLicenseActionDTO Action { get; set; }

	[JsonProperty("notificationMessage", NullValueHandling = NullValueHandling.Ignore)]
	public string NotificationMessage { get; set; }

	[JsonProperty("buyNowLink", NullValueHandling = NullValueHandling.Ignore)]
	public string BuyNowLink { get; set; }

}
