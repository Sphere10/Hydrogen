using System;
using System.Collections.Generic;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Message;
using Sphere10.Helium.Retry;

namespace Sphere10.Helium.Usage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var message = new TestMessage1 { FirstName = "stuff", Id = "1234" };

            var config = new BusConfiguration
            {
                EndpointType = EnumEndpointType.SendAndForget,
                IsPersisted = false
            };

            var bus = new BusSetup().Create(config);

            bus.SendAndForget("FarAway", message);

            var retries = new List<RetryCount>
            {
                new RetryCount(1, new TimeSpan(0, 0, 0, 10)),
                new RetryCount(2, new TimeSpan(0, 0, 0, 20)),
                new RetryCount(3, new TimeSpan(0, 0, 0, 30))
            };

            var x = new Retry.Retry();
            x.RetryCount(retries);
        }
    }

    public record TestMessage1 : IMessage
    {
        public string FirstName { get; init; }
        public string Id { get; set; }
    }
}
