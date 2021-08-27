using System;
using System.IO;
using Sphere10.Helium.Handler;
using Sphere10.Helium.TestPlugin3.Message;

namespace Sphere10.Helium.TestPlugin3.Handler {
	public class BlueHandler : IHandleMessage<BlueHandlerMessage2> {
		public void Handle(BlueHandlerMessage2 message) {

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
}