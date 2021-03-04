namespace Sphere10.Helium.Bus
{
    public class BusConfiguration : IBusConfiguration
    {
        public BusConfiguration() { }

        public string SourceEndpointName { get; set; }

        public EnumEndpointType EndpointType { get; set; }
    }
}
