using System;
using System.IO;
using Sphere10.Framework;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Usage {
	public class Program {

		public static void Main(string[] args) {
			Console.WriteLine("Hello World!");

			var bat = new Bat();

			ConsoleKeyInfo cki;
			do {
				cki = Console.ReadKey();

				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'a' || cki.KeyChar == 'A')) {

					Console.WriteLine("Create Message");
					var message = CreateMessage();

					Console.WriteLine($"Id={message.Id}");
					bat.AddMessageToQueue(message);
				}
				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'b' || cki.KeyChar == 'B')) {
					var message = (TestMessage1)bat.RetrieveMessageFromQueue();

					Console.WriteLine($"Id={message.Id}");
					Console.WriteLine($"Aa1={message.Aa1}");
				}
				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'c' || cki.KeyChar == 'C')) {
					Console.WriteLine("ClearAll");
					bat.ClearAll();
				}
			} while (cki.Key != ConsoleKey.Escape);

			Console.WriteLine("END!");
		}

		private static IMessage CreateMessage() {

			var inMessage = new TestMessage1 {
				Id = Guid.NewGuid().ToString(),
				Aa1 = $"Hello please work! 1",
				Aa2 = $"Hello please work! 2",
				Aa3 = $"Hello please work! 3",
				Aa4 = $"Hello please work! 4"
			};

			return inMessage;
		}

		//private static void MessageAdded(object sender, EventArgs e) {
		//	Console.WriteLine("The threshold was reached.");
		//}
	}

	[Serializable]
	public record TestMessage1 : IMessage {
		public string Id { get; set; }
		public string Aa1 { get; init; }
		public string Aa2 { get; init; }
		public string Aa3 { get; init; }
		public string Aa4 { get; init; }
	}

	public class Bat {

		private const string StrGuid = "997D1367-E7B0-46F0-B0A1-686DC0F15945";
		private const string TempQueueName = "Temp_AB3CB3F9-3EBC-46B3-877D-14AB5A7A7FD2_1";
		private readonly Guid _sameGuid = new Guid(StrGuid);
		private readonly string _queueTempPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "a");
		private readonly LocalQueue _localQueue;

		public Bat() {

			if (!Directory.Exists(_queueTempPath))
				Directory.CreateDirectory(_queueTempPath);

			var queueConfig = new QueueConfigDto {
				Path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName),
				TempDirPath = _queueTempPath,
				FileId = _sameGuid,
				TransactionalPageSizeBytes = 1 << 17, /*DefaultTransactionalPageSize = 1 << 17; => 132071 ~ 128 KB*/
				MaxStorageSizeBytes = 1 << 21, /*2097152 ~ 2MB*/
				FileMemoryCacheBytes = 1 << 20, /*1048576 ~ 1MB*/
				ClusterSize = 1 << 9, /*512 ~ 500 KB*/
				MaxItems = 5,
				ReadOnly = false
			};

			_localQueue = new LocalQueue(queueConfig);

			if (_localQueue.RequiresLoad)
				_localQueue.Load();
			
			_localQueue.Committed += MessageAdded;
			//_localQueue.Added += MessageAdded; this event fires when a message is added. Cannot use it in the context of a Transaction-Scope Commit//
		}

		public void ClearAll() {
			_localQueue.Clear();
		}

		public void AddMessageToQueue(IMessage message) {

			//////localQueue.DeleteMessage(inMessage);
			_localQueue.AddMessage(message);
			//////var outMessage = localQueue.RetrieveMessage();
			//////localQueue.DeleteMessage(inMessage);
		}

		public void DeleteMessageFromQueue(IMessage message) {
			_localQueue.DeleteMessage(message);
		}

		public IMessage RetrieveMessageFromQueue() {
			var outMessage = _localQueue.RetrieveMessage();

			return outMessage;
		}

		private void MessageAdded(object sender) {
			Console.WriteLine("Commit event has fired.");
			//var message = (TestMessage1)RetrieveMessageFromQueue();

			//if (message is not null) {
			//	Console.WriteLine($"Id={message.Id}");
			//	Console.WriteLine($"Aa1={message.Aa1}");
			//} else Console.WriteLine("Message is null.");
		}
	}
}
