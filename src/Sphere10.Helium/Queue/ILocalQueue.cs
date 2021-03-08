using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue
{
    public interface ILocalQueue
    {
        public string FileName { get; set; }

        void FirstIn(string destination, IMessage message);
    }
}
