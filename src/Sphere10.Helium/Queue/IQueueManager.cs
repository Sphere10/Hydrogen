using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue
{
    public interface IQueueManager
    {
        void FirstIn(IMessage message);

        void LastOut(IMessage message);

        void TakeThisMessageOffQueue(IMessage message);

        void PersistQueue();
    }
}
