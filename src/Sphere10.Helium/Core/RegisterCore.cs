using System;
using System.Collections.Generic;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Handler;
using Sphere10.Helium.Message;
using Sphere10.Helium.Saga;

namespace Sphere10.Helium.Core
{
    public class RegisterCore : IRegisterCoreTypes
    {
        public IList<IHandleMessage<IMessage>> HandlerList { get; set; }

        public IList<IHandleTimeout<IMessage>> TimeoutList { get; set; }

        public IList<IStartSaga<IMessage>> StartSagaList { get; set; }

        public IList<IEndSaga<IMessage>> EndSagaList { get; set; }

        public IBus Bus { get; set; }

        public IConfigureThisEndpoint EndpointConfiguration { get; set; }

        public InitResultDto InitializeHelium()
        {
            if ($"Error in Registering and Instantiating types" is string)
            {
                return new InitResultDto(false, "All the errors go here.");
            }



            return new InitResultDto(true, string.Empty);

            throw new NotImplementedException();
        }
    }
}
