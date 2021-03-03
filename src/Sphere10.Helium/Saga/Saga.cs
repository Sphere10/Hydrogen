using System;
using Sphere10.Helium.Bus;

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

        protected void RequestTimeout<TTimeoutMessageType>(DateTime at) where TTimeoutMessageType : new() => RequestTimeout<TTimeoutMessageType>(at, new TTimeoutMessageType());

        protected void RequestTimeout<TTimeoutMessageType>(DateTime at, TTimeoutMessageType timeoutMessage)
        {
            if (at.Kind == DateTimeKind.Unspecified) throw new InvalidOperationException("The Kind for DateTime 'at' must be specified. Cannot Proceed!");

            VerifySagaCanHandleTimeout(timeoutMessage);

            SetTimeoutHeaders(timeoutMessage);

            Bus.Defer(at, timeoutMessage);
        }

        protected void RequestTimeout<TTimeoutMessageType>(TimeSpan within) where TTimeoutMessageType : new() => this.RequestTimeout<TTimeoutMessageType>(within, new TTimeoutMessageType());
        
        protected void RequestTimeout<TTimeoutMessageType>(TimeSpan within, TTimeoutMessageType timeoutMessage)
        {
            VerifySagaCanHandleTimeout<TTimeoutMessageType>(timeoutMessage);
            SetTimeoutHeaders(timeoutMessage);
            Bus.Defer(within, timeoutMessage);
        }

        private void SetTimeoutHeaders(object toSend)
        {
            Bus.SetMessageHeader(toSend, "SagaId", Entity.Id.ToString());
            Bus.SetMessageHeader(toSend, "IsSagaTimeoutMessage", bool.TrueString);
            Bus.SetMessageHeader(toSend, "SagaType", GetType().AssemblyQualifiedName);
        }

        protected virtual void ReplyToOriginator(object message)
        {
            if (string.IsNullOrEmpty(this.Entity.Originator))
                throw new Exception("Cannot proceed! Entity.Originator is null.");
            
            Bus.SetMessageHeader(message, "$.temporary.ReplyToOriginator", nameof(ReplyToOriginator));
            Bus.Send(Entity.Originator, Entity.OriginalMessageId, message);
        }

        protected virtual void MarkAsComplete() => Completed = true;
    }
}
