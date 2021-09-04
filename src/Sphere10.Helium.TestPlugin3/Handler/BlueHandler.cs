using System;
using System.IO;
using Sphere10.Helium.Handler;
using Sphere10.Helium.TestPlugin3.Message;

namespace Sphere10.Helium.TestPlugin3.Handler {
	public class BlueHandler : Helium.Handler.Handler, IHandleMessage<BlueHandlerMessage2> {
		public void Handle(BlueHandlerMessage2 message) {

			var fileName = $"{nameof(BlueHandler)}.txt";

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
}