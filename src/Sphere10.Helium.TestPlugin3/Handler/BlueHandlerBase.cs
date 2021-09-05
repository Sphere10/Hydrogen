using System;
using System.IO;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Handle;
using Sphere10.Helium.TestPlugin3.Message;

namespace Sphere10.Helium.TestPlugin3.Handler {
	public class BlueHandlerBase : HandlerBase, IHandleMessage<BlueHandlerMessage2> {

		public BlueHandlerBase(IBus bus) : base(bus) {
		}

		public void Handle(BlueHandlerMessage2 message) {

			var fileName = $"{nameof(BlueHandlerBase)}.txt";

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