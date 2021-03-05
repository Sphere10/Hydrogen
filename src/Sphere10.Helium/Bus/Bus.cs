using Sphere10.Helium.Message;
using System;
using Sphere10.Helium.Queue;
using Sphere10.Helium.Timeout;

namespace Sphere10.Helium.Bus
{
    public class Bus : IBus
    {
        private readonly IMessageHeader _messageHeader;
        private readonly ITimeoutManager _timeoutManager;
        private readonly IQueueManager _queueManager;

        public Bus(IQueueManager queueManager, IMessageHeader messageHeader, ITimeoutManager timeoutManager)
        {
            _messageHeader = messageHeader;
            _timeoutManager = timeoutManager;
            _queueManager = queueManager;
        }

        public ICallback SendLocal<TK>(IMessage message, IMessageHeader missingName)
        {
            throw new NotImplementedException();
        }

        public ICallback RegisterTimeout(TimeSpan delay, IMessage message)
        {
            throw new NotImplementedException();
        }

        public ICallback RegisterTimeout(DateTime processAt, IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Reply<TK>(Action<TK> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public void Return<TK>(TK errorEnum)
        {
            throw new NotImplementedException();
        }

        public void SendAndForget(string destination, IMessage message)
        {
            throw new NotImplementedException();
        }

        public void SendAndForget(string destination, IMessage message, IMessageHeader messageHeader)
        {
            throw new NotImplementedException();
        }

        public ICallback SendAndResponse(string destination, IMessage message)
        {
            throw new NotImplementedException();
        }

        public ICallback SendAndResponse(string destination, IMessage message, IMessageHeader messageHeader)
        {
            throw new NotImplementedException();
        }

        public ICallback SendLocal<TK>(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TK>()
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TK>()
        {
            throw new NotImplementedException();
        }
    }
}
