using System;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Handler;
using Sphere10.Helium.Message;
using Sphere10.Helium.Saga;

namespace Sphere10.Helium.Usage
{
    public class BlueSaga : Saga<BlueSagaData>, 
        IStartSaga<BlueSagaStart>,
        IEndSaga<BlueSagaEnd>,
        IHandleMessage<BlueSagaWorkflow1>,
        IHandleMessage<BlueSagaWorkflow2>
    {
        public new IBus Bus;
        
        public BlueSaga(IBus bus)
        {
            Bus = bus;
        }
        
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BlueSagaData> mapper)
        {
            mapper.ConfigureMapping<BlueSagaStart>(m => m.Id).ToSaga(s => s.Id);

            throw new NotImplementedException();
        }

        public void Handle(BlueSagaStart message)
        {
            Data.Id = Guid.NewGuid();
            
            throw new NotImplementedException();
        }

        public void Handle(BlueSagaEnd message)
        {
            throw new NotImplementedException();
        }

        public void Handle(BlueSagaWorkflow1 message)
        {
            throw new NotImplementedException();
        }

        public void Handle(BlueSagaWorkflow2 message)
        {
            throw new NotImplementedException();
        }
    }

    public class BlueSagaData : ISagaDataForSaga
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
    }

    public class BlueSagaStart : ICommand
    {
        public string Id { get; set; }
    }

    public class BlueSagaEnd : ICommand
    {
        public string Id { get; set; }
    }

    public class BlueSagaWorkflow1 : IMessage
    {
        public string Id { get; set; }
    }

    public class BlueSagaWorkflow2 : IMessage
    {
        public string Id { get; set; }
    }

}
