using System;
using System.IO;
using Sphere10.Helium.Handler;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.TestPlugin1 {
	public class BlueHandler : IHandleMessage<BlueHandlerMessage> {
		//private readonly IBus _bus;

		//public BlueHandler(IBus bus) {
		//	_bus = bus;
		//}

		public void Handle(BlueHandlerMessage message) {
			const string path = @"C:\Temp\MyTest.txt";

			if (File.Exists(path)) File.Delete(path);

			using var sw = File.CreateText(path);

			sw.WriteLine("Hello");
			sw.WriteLine("And");
			sw.WriteLine("Welcome");
			sw.WriteLine($"=>{DateTime.Now}");
			sw.WriteLine($"=>Id={message.Id}");
		}
	}

	[Serializable]
	public class BlueHandlerMessage : IMessage {
		public string Id { get; set; }
	}
}
