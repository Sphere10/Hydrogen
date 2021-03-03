namespace Sphere10.Helium.Bus
{
    public interface IBusConfiguration
    {
        public string EndpointName { get; set; }
        public EnumEndpointType EndpointType { get; set; }
    }
}