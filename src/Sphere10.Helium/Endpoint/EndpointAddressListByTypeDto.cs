namespace Sphere10.Helium.Endpoint {
	public record EndpointAddressListByTypeDto {

		public EndpointAddressListByTypeDto(string endpointAddress, string type) {
			EndpointAddress = endpointAddress;
			Type = type;
		}

		public string EndpointAddress { get; set;}
		public string Type { get; set; }
	}
}
