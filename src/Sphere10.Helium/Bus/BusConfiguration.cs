namespace Sphere10.Helium.Bus
{
    public class BusConfiguration : IBusConfiguration
    {
        public BusConfiguration() { }

        public string EndpointName { get; set; }

        public EnumEndpointType EndpointType { get; set; }
    }
}
