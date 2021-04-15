using Sphere10.Framework;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue {
	public interface IHeliumQueue : ITransactionalList<IMessage> {

		void FirstIn(string destination, IMessage message);
	}
}
