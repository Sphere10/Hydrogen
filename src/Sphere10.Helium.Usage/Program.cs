using Sphere10.Helium.Bus;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Usage
{

	public class Program
    {
        public static void Main(string[] args)
        {
            var message = new TestMessage1 { FirstName = "stuff", Id = "1234" };

            var config = new EndpointConfiguration
            {
                EndpointType = EnumEndpointType.SendAndForget,
                IsPersisted = false
            };

            var bus = new BusSetup().Create(config);

            bus.SendAndForget("FarAway", message);
        }
    }

    public record TestMessage1 : IMessage
    {
        public string FirstName { get; init; }
        public string Id { get; set; }
    }
}
