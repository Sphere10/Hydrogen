using Newtonsoft.Json;

namespace Hydrogen.Application;

public class ProductLicenseActivationDTO {

	[JsonProperty("authority")]
	public ProductLicenseAuthorityDTO Authority { get; set; }

	[JsonProperty("license")]
	public SignedItem<ProductLicenseDTO> License { get; set; }

	[JsonProperty("command")]
	public SignedItem<ProductLicenseCommandDTO> Command { get; set; }

}