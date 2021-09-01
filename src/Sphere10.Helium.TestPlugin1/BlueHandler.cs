using System;
using System.IO;
using Sphere10.Helium.Handler;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.TestPlugin1 {
	public class BlueHandler : IHandleMessage<BlueHandlerMessage> {
		private readonly IBlueBat _blueBat;
		private readonly IGreenBat _greenBat;

		public BlueHandler() { }
	
		public BlueHandler(IBlueBat blueBat, IGreenBat greenBat) {
			_blueBat = blueBat;
			_greenBat = greenBat;
		}

		public void Handle(BlueHandlerMessage message) {
			var fileName = $"{message.MessageNumber}_{message.MessageName}_{message.Id}.txt";
			var path = $@"C:\Temp\{fileName}";
			message.EndDateTime = DateTime.Now.Ticks;

			var ts = TimeSpan.FromTicks(message.EndDateTime - message.StartDateTime);
			message.ProcessingTimeMSec = ts.Milliseconds;

			var startDateTime = new DateTime(message.StartDateTime);
			var endDateTime = new DateTime(message.EndDateTime);
			
			if (File.Exists(path)) File.Delete(path);

			using var sw = File.CreateText(path);

			sw.WriteLine("Hello");
			sw.WriteLine("And");
			sw.WriteLine("Welcome");
			sw.WriteLine($"StartDateTime={startDateTime:dd/MM/yyyy HH:mm:ss.ffffff}");
			sw.WriteLine($"EndDateTime={endDateTime:dd/MM/yyyy HH:mm:ss.ffffff}");
			sw.WriteLine($"ProcessingTimeInMillisecond={message.ProcessingTimeMSec}");
		}
	}

	[Serializable]
	public class BlueHandlerMessage : IMessage {
		public string Id { get; set; }
		public int MessageNumber { get; set; }
		public string MessageName { get; set; }
		public long StartDateTime { get; set; }
		public long EndDateTime { get; set; }
		public int ProcessingTimeMSec { get; set; }
	}

	public interface IBlueBat {
		int GetBlueBat1(string input);
	}

	public class BlueBat : IBlueBat {
		public int GetBlueBat1(string input) {
			return 1;
		}
	}

	public interface IGreenBat {
		int GetGreenBat1(string input);
	}

	public class GreenBat : IGreenBat {
		public int GetGreenBat1(string input) {
			return 2;
		}
	}
}