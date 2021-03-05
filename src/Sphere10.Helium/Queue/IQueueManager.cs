using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue
{
    public interface IQueueManager
    {
        void PutMessageInQueue(IMessage message);

        void TakeMessageFromQueue(IMessage message);

        void TakeThisMessageFromQueue(IMessage message);

        void PerssitQueue();
    }
}
