using System;
using System.IO;
using Sphere10.Helium.Handler;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.TestPlugin1 {
	public class BlueHandler : IHandleMessage<BlueHandlerMessage> {

		public void Handle(BlueHandlerMessage message) {
			var fileName = $"T1_{nameof(BlueHandler)}.txt";

			var path = $@"C:\Temp\{fileName}";

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
