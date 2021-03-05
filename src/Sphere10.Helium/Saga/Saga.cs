using System;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Saga
{
    public abstract class Saga
    {
        private IBus _bus;

        public ISagaDataForSaga Entity { get; set; }

        protected internal abstract void ConfigureHowToFindSaga(IFindSaga sagaFindMap);

        public IBus Bus
        {
            get => _bus ?? throw new InvalidOperationException("Cannot proceed! IBus not instantiated. Do NOT use your own bus here!");
            set => _bus = value;
        }

        public bool Completed { get; private set; }

        private void VerifySagaCanHandleTimeout<TTimeoutMessageType>(TTimeoutMessageType timeoutMessage)
        {
            if (!(timeoutMessage is IHandleTimeout<TTimeoutMessageType>))
            {
                throw new Exception(
                    $"'{(object)GetType().Name}' Cannot proceed! Timeout for '{(object)timeoutMessage}' must implement 'IHandleTimeouts<{(object)typeof(TTimeoutMessageType).FullName}>'");
            }
        }

        protected void RequestTimeout<TTimeoutMessageType>(DateTime at, IMessage timeoutMessage)
        {
            if (at.Kind == DateTimeKind.Unspecified) throw new InvalidOperationException("The Kind for DateTime 'at' must be specified. Cannot Proceed!");

            VerifySagaCanHandleTimeout(timeoutMessage);

            Bus.RegisterTimeout(at, timeoutMessage);
        }

        protected void RequestTimeout<IMessage>(TimeSpan within, IMessage timeoutMessage)
        {
            VerifySagaCanHandleTimeout<IMessage>(timeoutMessage);
            var callback = Bus.RegisterTimeout(within, timeoutMessage as Message.IMessage);
        }
        
        protected virtual void ReplyToOriginator(IMessage message)
        {
            if (string.IsNullOrEmpty(this.Entity.Originator))
                throw new Exception("Cannot proceed! Entity.Originator is null.");
            
            //BusSetup.SendAndForget(Entity.Originator, message, new NotI{} as IMessageHeader );
        }

        protected virtual void MarkAsComplete() => Completed = true;
    }
}
