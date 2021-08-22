using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Sphere10.Framework;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.TestPlugin1 {
	public class Program {

		public static void Main(string[] args) {
			Console.WriteLine("Hello World!");

			var localQueueProcessor = new AsLocalQueueProcessor();

			ConsoleKeyInfo cki;
			do {
				cki = Console.ReadKey();

				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'a' || cki.KeyChar == 'A')) {

					Console.WriteLine("Create Message");
					var message = CreateMessage();

					Console.WriteLine($"Id={message.Id}");
					localQueueProcessor.AddMessageToQueue(message);
				}
				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'b' || cki.KeyChar == 'B')) {
					var message = (TestMessage1)localQueueProcessor.RetrieveMessageFromQueue();

					Console.WriteLine($"Id={message.Id}");
					Console.WriteLine($"Aa1={message.Aa1}");
				}
				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'c' || cki.KeyChar == 'C')) {
					Console.WriteLine("ClearAll");
					Console.WriteLine($"Start count={localQueueProcessor.CountLocal()}");
					localQueueProcessor.ClearAll();
					Console.WriteLine($"End count={localQueueProcessor.CountLocal()}");
				}
				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'd' || cki.KeyChar == 'D')) {
					var taskA = new Task(() => {
						Console.WriteLine("In taskA.");
						var message = CreateMessage();
						localQueueProcessor.AddMessageToQueue(message);
						Console.WriteLine($"Id={message.Id}");
						Console.WriteLine("Out taskA.");
					});

					//var taskB = new Task(() => {
					//	Console.WriteLine("In taskB.");
					//	var message = localQueueProcessor.RetrieveMessageFromQueue();
					//	Console.WriteLine($"B=>{message.Id}");
					//	Console.WriteLine("Out taskB.");
					//});

					var taskC = new Task(() => {
						Console.WriteLine("In taskC.");
						var message = CreateMessage();
						localQueueProcessor.AddMessageToQueue(message);
						Console.WriteLine($"Id={message.Id}");
						Console.WriteLine("Out taskC.");
					});

					//var taskD = new Task(() => {
					//	Console.WriteLine("In taskD.");
					//	var message = localQueueProcessor.RetrieveMessageFromQueue();
					//	Console.WriteLine($"D=>{message.Id}");
					//	Console.WriteLine("Out taskD.");
					//});

					// Start the task.
					taskA.Start();
					//taskB.Start();
					taskC.Start();
					//taskD.Start();

					taskA.Wait();
					//taskB.Wait();
					taskC.Wait();
					//taskD.Wait();
				}
			} while (cki.Key != ConsoleKey.Escape);

			Console.WriteLine("END!");
		}

		private static IMessage CreateMessage() {

			var inMessage = new TestMessage1 {
				Id = Guid.NewGuid().ToString(),
				Aa1 = $"Hello please work! 1{DateTime.Now:F}",
				Aa2 = $"Hello please work! 2{DateTime.Now:F}",
				Aa3 = $"Hello please work! 3{DateTime.Now:F}",
				Aa4 = $"Hello please work! 4{DateTime.Now:F}"
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

	public class AsLocalQueueProcessor {

		private readonly LocalQueueSettings _queueSettings;

		private const string StrGuid = "997D1367-E7B0-46F0-B0A1-686DC0F15945";
		private const string TempQueueName = "Temp_AB3CB3F9-3EBC-46B3-877D-14AB5A7A7FD2_1";
		private readonly Guid _sameGuid = new Guid(StrGuid);
		private readonly string _queueTempDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "a");

		public IHeliumQueue LocalQueue;
		public IHeliumQueue ProcessingQueue;

		public AsLocalQueueProcessor() {

			_queueSettings = new LocalQueueSettings();

			if (!Directory.Exists(_queueSettings.TempDirPath))
				Directory.CreateDirectory(_queueSettings.TempDirPath);

			if (File.Exists(_queueSettings.Path))
				File.Delete(_queueSettings.Path);

			if (LocalQueue.RequiresLoad)
				LocalQueue.Load();

			//if (!Directory.Exists(_queueTempDir))
			//	Directory.CreateDirectory(_queueTempDir);

			//var queuePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName);
			
			//Debug.Assert(!Tools.FileSystem.DirectoryContainsFiles(_queueTempDir));

			//if(File.Exists(queuePath)) File.Delete(queuePath);

			//_queueSettings = new LocalQueueSettings();

			//_localQueue = SetupLocalQueue();
			//_processingQueue = SetupProcessingQueue();

			if (LocalQueue.RequiresLoad)
				LocalQueue.Load();

			LocalQueue.Committed += MessageAdded;
		}

		public int CountLocal() {
			return LocalQueue.Count;
		}

		public void ClearAll() {
			LocalQueue.Clear();
		}

		public void AddMessageToQueue(IMessage message) {

			using var txnScope = new FileTransactionScope(_queueSettings.TempDirPath, ScopeContextPolicy.None);
			txnScope.BeginTransaction();
			txnScope.EnlistFile(LocalQueue, false);

			using (LocalQueue.EnterWriteScope()) {
				LocalQueue.Add(message);
				LocalQueue.Add(message);
				LocalQueue.Add(message);
				LocalQueue.Add(message);
				LocalQueue.Add(message);
				LocalQueue.Add(message);
				LocalQueue.Add(message);
				LocalQueue.Add(message);
				LocalQueue.Add(message);
				LocalQueue.Add(message);
				LocalQueue.Add(message);
			}

			txnScope.Commit();

			//////localQueue.DeleteMessage(inMessage);
			//_localQueue.AddMessage(message);
			//////var outMessage = localQueue.ReadMessage();
			//////localQueue.DeleteMessage(inMessage);
		}

		public void DeleteMessageFromQueue(IMessage message) {
			LocalQueue.DeleteMessage(message);
		}

		public IMessage RetrieveMessageFromQueue() {
			if (LocalQueue.Count == 0)
				throw new InvalidOperationException("CRITICAL ERROR: LocalQueue is empty and should not be empty. Message missing cannot proceed.");

			using var txnScope = new FileTransactionScope(_queueSettings.TempDirPath, ScopeContextPolicy.None);
			txnScope.BeginTransaction();
			txnScope.EnlistFile(LocalQueue, false);

			var localQueueMessage = LocalQueue[^1];

			LocalQueue.RemoveAt(^1);

			txnScope.Commit();

			return localQueueMessage;

			//var outMessage = _localQueue.ReadMessage();
			//return outMessage;
		}

		private void MessageAdded(object sender) {
			Console.WriteLine("Commit event has fired.");
			//var message = (TestMessage1)RetrieveMessageFromQueue();

			//if (message is not null) {
			//	Console.WriteLine($"Id={message.Id}");
			//	Console.WriteLine($"Aa1={message.Aa1}");
			//} else Console.WriteLine("Message is null.");
		}

		//private IHeliumQueue SetupLocalQueue() {
		//	return _localQueue ??= new LocalQueue(_queueSettings);
		//}

		private IHeliumQueue SetupProcessingQueue() {
			return ProcessingQueue ??= new ProcessingQueue(_queueSettings);
		}
	}
}