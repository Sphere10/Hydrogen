namespace Sphere10.Helium.Bus
{
    public static class Bus
    {
        public static IBus Create(BusConfiguration busConfiguration)
        {
            if (busConfiguration.EndpointType == EnumEndpointType.SendAndForget ||
                busConfiguration.EndpointType == EnumEndpointType.SendAndResponse)
            {


                return new BusConfiguration() as IBus;
            }

            //var type = Type.GetType("BusConfiguration");
            //var typeCtor = type.GetConstructor(Type.EmptyTypes);

            //var endpointName = type.GetProperty("EndpointName");
            //var endpointType = type.GetProperty("EndpointType");


            //busConfiguration.GetSettings().Set("Endpoint.SendOnly", (object)true);
            //Configure configure = busConfiguration.BuildConfiguration();
            //configure.Initialize();
            //return configure.Builder.Build<ISendOnlyBus>();

            return null;
        }
    }
}
