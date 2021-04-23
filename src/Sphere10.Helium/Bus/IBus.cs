using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Bus {
	public interface IBus : ISendOnlyBus {
		void Subscribe<Tk>();

		void Unsubscribe<Tk>();

		ICallback SendLocal(IMessage message);

		ICallback SendLocal<Tk>(IMessage message, IMessageHeader messageHeader);

		ICallback RegisterTimeout(TimeSpan delay, IMessage message);

		ICallback RegisterTimeout(DateTime processAt, IMessage message);

		void Reply<Tk>(Action<Tk> messageConstructor);

		void Return<Tk>(Tk errorEnum);

	}
}
