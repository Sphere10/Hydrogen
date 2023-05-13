// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
