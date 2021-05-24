//using System;
//using System.IO;
//using System.Threading.Tasks;
//using Sphere10.Framework;
//using Sphere10.Helium.Message;
//using Sphere10.Helium.Processor;
//using Sphere10.Helium.Queue;

//namespace Sphere10.Helium.Usage {
//	public class Program {

//		public static void Main(string[] args) {
//			Console.WriteLine("Hello World!");

//			var localQueueProcessor = new AsLocalQueueProcessor();

//			ConsoleKeyInfo cki;
//			do {
//				cki = Console.ReadKey();

//				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'a' || cki.KeyChar == 'A')) {

//					Console.WriteLine("Create Message");
//					var message = CreateMessage();

//					Console.WriteLine($"Id={message.Id}");
//					localQueueProcessor.AddMessageToQueue(message);
//				}
//				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'b' || cki.KeyChar == 'B')) {
//					var message = (TestMessage1)localQueueProcessor.RetrieveMessageFromQueue();

//					Console.WriteLine($"Id={message.Id}");
//					Console.WriteLine($"Aa1={message.Aa1}");
//				}
//				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'c' || cki.KeyChar == 'C')) {
//					Console.WriteLine("ClearAll");
//					localQueueProcessor.ClearAll();
//				}
//				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'd' || cki.KeyChar == 'D')) {
//					var taskA = new Task(() => {
//						Console.WriteLine("In taskA.");
//						var message = CreateMessage();
//						localQueueProcessor.AddMessageToQueue(message);
//						Console.WriteLine($"Id={message.Id}");
//						Console.WriteLine("Out taskA.");
//					});

//					var taskB = new Task(() => {
//						Console.WriteLine("In taskB.");
//						var message = localQueueProcessor.RetrieveMessageFromQueue();
//						Console.WriteLine($"B=>{message.Id}");
//						Console.WriteLine("Out taskB.");
//					});

//					var taskC = new Task(() => {
//						Console.WriteLine("In taskC.");
//						var message = CreateMessage();
//						localQueueProcessor.AddMessageToQueue(message);
//						Console.WriteLine($"Id={message.Id}");
//						Console.WriteLine("Out taskC.");
//					});

//					var taskD = new Task(() => {
//						Console.WriteLine("In taskD.");
//						var message = localQueueProcessor.RetrieveMessageFromQueue();
//						Console.WriteLine($"D=>{message.Id}");
//						Console.WriteLine("Out taskD.");
//					});

//					Start the task.
//				   taskA.Start();
//					taskB.Start();
//					taskC.Start();
//					taskD.Start();

//					taskA.Wait();
//					taskB.Wait();
//					taskC.Wait();
//					taskD.Wait();
//				}
//			} while (cki.Key != ConsoleKey.Escape);

//			Console.WriteLine("END!");
//		}

//		private static IMessage CreateMessage() {

//			var inMessage = new TestMessage1 {
//				Id = Guid.NewGuid().ToString(),
//				Aa1 = $"Hello please work! 1",
//				Aa2 = $"Hello please work! 2",
//				Aa3 = $"Hello please work! 3",
//				Aa4 = $"Hello please work! 4"
//			};

//			return inMessage;
//		}

//		private static void MessageAdded(object sender, EventArgs e) {
//			Console.WriteLine("The threshold was reached.");
//		}
//	}

//	[Serializable]
//	public record TestMessage1 : IMessage {
//		public string Id { get; set; }
//		public string Aa1 { get; init; }
//		public string Aa2 { get; init; }
//		public string Aa3 { get; init; }
//		public string Aa4 { get; init; }
//	}

//	public class AsLocalQueueProcessor {

//		private QueueConfigDto _queueConfigDto;

//		private const string StrGuid = "997D1367-E7B0-46F0-B0A1-686DC0F15945";
//		private const string TempQueueName = "Temp_AB3CB3F9-3EBC-46B3-877D-14AB5A7A7FD2_1";
//		private readonly Guid _sameGuid = new Guid(StrGuid);
//		private readonly string _queueTempPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "a");

//		private IHeliumQueue _localQueue;
//		private IHeliumQueue _processingQueue;

//		public AsLocalQueueProcessor() {

//			if (!Directory.Exists(_queueTempPath))
//				Directory.CreateDirectory(_queueTempPath);

//			var queueConfig = new QueueConfigDto {
//				Path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName),
//				TempDirPath = _queueTempPath,
//				FileId = _sameGuid,
//				TransactionalPageSizeBytes = 1 << 17, /*DefaultTransactionalPageSize = 1 << 17; => 132071 ~ 128 KB*/
//				MaxStorageSizeBytes = 1 << 21, /*2097152 ~ 2MB*/
//				FileMemoryCacheBytes = 1 << 20, /*1048576 ~ 1MB*/
//				ClusterSize = 1 << 9, /*512 ~ 500 KB*/
//				MaxItems = 5,
//				ReadOnly = false
//			};

//			_queueConfigDto = queueConfig;

//			_localQueue = SetupLocalQueue();
//			_processingQueue = SetupProcessingQueue();

//			_localQueue.Committed += MessageAdded;
//		}

//		public void ClearAll() {
//			_localQueue.Clear();
//		}

//		public void AddMessageToQueue(IMessage message) {

//			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath, ScopeContextPolicy.None);

//			txnScope.EnlistFile(_localQueue);

//			using (_localQueue.EnterWriteScope()) {
//				_localQueue.Add(message);
//			}

//			txnScope.Commit();

//			////localQueue.DeleteMessage(inMessage);
//			_localQueue.AddMessage(message);
//			////var outMessage = localQueue.RetrieveMessage();
//			////localQueue.DeleteMessage(inMessage);
//		}

//		public void DeleteMessageFromQueue(IMessage message) {
//			_localQueue.DeleteMessage(message);
//		}

//		public IMessage RetrieveMessageFromQueue() {
//			if (_localQueue.Count == 0)
//				throw new InvalidOperationException("CRITICAL ERROR: LocalQueue is empty and should not be empty. Message missing cannot proceed.");

//			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath, ScopeContextPolicy.None);

//			txnScope.EnlistFile(_localQueue);

//			var localQueueMessage = _localQueue[^1];

//			_localQueue.RemoveAt(^1);

//			txnScope.Commit();

//			return localQueueMessage;

//			var outMessage = _localQueue.RetrieveMessage();
//			return outMessage;
//		}

//		private void MessageAdded(object sender) {
//			Console.WriteLine("Commit event has fired.");
//			var message = (TestMessage1)RetrieveMessageFromQueue();

//			if (message is not null) {
//				Console.WriteLine($"Id={message.Id}");
//				Console.WriteLine($"Aa1={message.Aa1}");
//			} else Console.WriteLine("Message is null.");
//		}

//		private IHeliumQueue SetupLocalQueue() {
//			return _localQueue ??= new LocalQueue(_queueConfigDto);
//		}

//		private IHeliumQueue SetupProcessingQueue() {
//			return _processingQueue ??= new ProcessingQueue(_queueConfigDto);
//		}
//	}
//}


//using System;
//using System.IO;
//using System.Threading.Tasks;
//using Sphere10.Framework;
//using Sphere10.Helium.Message;
//using Sphere10.Helium.Queue;

//namespace Sphere10.Helium.Usage {
//	public class Program {

//		public static void Main(string[] args) {
//			Console.WriteLine("Hello World!");

//			var localQueueProcessor = new AsLocalQueueProcessor();

//			ConsoleKeyInfo cki;
//			do {
//				cki = Console.ReadKey();

//				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'a' || cki.KeyChar == 'A')) {

//					Console.WriteLine("Create Message");
//					var message = CreateMessage();

//					Console.WriteLine($"Id={message.Id}");
//					localQueueProcessor.AddMessageToQueue(message);
//				}
//				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'b' || cki.KeyChar == 'B')) {
//					var message = (TestMessage1)localQueueProcessor.RetrieveMessageFromQueue();

//					Console.WriteLine($"Id={message.Id}");
//					Console.WriteLine($"Aa1={message.Aa1}");
//				}
//				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'c' || cki.KeyChar == 'C')) {
//					Console.WriteLine("ClearAll");
//					localQueueProcessor.ClearAll();
//				}
//				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'd' || cki.KeyChar == 'D')) {
//					var taskA = new Task(() => {
//						Console.WriteLine("In taskA.");
//						var message = CreateMessage();
//						localQueueProcessor.AddMessageToQueue(message);
//						Console.WriteLine($"Id={message.Id}");
//						Console.WriteLine("Out taskA.");
//					});

//					var taskB = new Task(() => {
//						Console.WriteLine("In taskB.");
//						var message = localQueueProcessor.RetrieveMessageFromQueue();
//						Console.WriteLine($"B=>{message.Id}");
//						Console.WriteLine("Out taskB.");
//					});

//					var taskC = new Task(() => {
//						Console.WriteLine("In taskC.");
//						var message = CreateMessage();
//						localQueueProcessor.AddMessageToQueue(message);
//						Console.WriteLine($"Id={message.Id}");
//						Console.WriteLine("Out taskC.");
//					});

//					var taskD = new Task(() => {
//						Console.WriteLine("In taskD.");
//						var message = localQueueProcessor.RetrieveMessageFromQueue();
//						Console.WriteLine($"D=>{message.Id}");
//						Console.WriteLine("Out taskD.");
//					});

//					// Start the task.
//					taskA.Start();
//					taskB.Start();
//					taskC.Start();
//					taskD.Start();

//					taskA.Wait();
//					taskB.Wait();
//					taskC.Wait();
//					taskD.Wait();
//				}
//			} while (cki.Key != ConsoleKey.Escape);

//			Console.WriteLine("END!");
//		}

//		private static IMessage CreateMessage() {

//			var inMessage = new TestMessage1 {
//				Id = Guid.NewGuid().ToString(),
//				Aa1 = $"Hello please work! 1",
//				Aa2 = $"Hello please work! 2",
//				Aa3 = $"Hello please work! 3",
//				Aa4 = $"Hello please work! 4"
//			};

//			return inMessage;
//		}

//		//private static void MessageAdded(object sender, EventArgs e) {
//		//	Console.WriteLine("The threshold was reached.");
//		//}
//	}

//	[Serializable]
//	public record TestMessage1 : IMessage {
//		public string Id { get; set; }
//		public string Aa1 { get; init; }
//		public string Aa2 { get; init; }
//		public string Aa3 { get; init; }
//		public string Aa4 { get; init; }
//	}

//	public class AsLocalQueueProcessor {

//		private const string StrGuid = "997D1367-E7B0-46F0-B0A1-686DC0F15945";
//		private const string TempQueueName = "Temp_AB3CB3F9-3EBC-46B3-877D-14AB5A7A7FD2_1";
//		private readonly Guid _sameGuid = new Guid(StrGuid);
//		private readonly string _queueTempPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "a");
//		private readonly LocalQueue _localQueue;

//		public AsLocalQueueProcessor() {

//			if (!Directory.Exists(_queueTempPath))
//				Directory.CreateDirectory(_queueTempPath);

//			var queueConfig = new QueueConfigDto {
//				Path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName),
//				TempDirPath = _queueTempPath,
//				FileId = _sameGuid,
//				TransactionalPageSizeBytes = 1 << 17, /*DefaultTransactionalPageSize = 1 << 17; => 132071 ~ 128 KB*/
//				MaxStorageSizeBytes = 1 << 21, /*2097152 ~ 2MB*/
//				FileMemoryCacheBytes = 1 << 20, /*1048576 ~ 1MB*/
//				ClusterSize = 1 << 9, /*512 ~ 500 KB*/
//				MaxItems = 5,
//				ReadOnly = false
//			};

//			_localQueue = new LocalQueue(queueConfig);

//			//if (_localQueue.RequiresLoad)
//			//	_localQueue.Load();

//			_localQueue.Committed += MessageAdded;
//			//_localQueue.Added += MessageAdded; this event fires when a message is added. Cannot use it in the context of a Transaction-Scope Commit//
//		}

//		public void ClearAll() {
//			_localQueue.Clear();
//		}

//		public void AddMessageToQueue(IMessage message) {

//			//////localQueue.DeleteMessage(inMessage);
//			_localQueue.AddMessage(message);
//			//////var outMessage = localQueue.RetrieveMessage();
//			//////localQueue.DeleteMessage(inMessage);
//		}

//		public void DeleteMessageFromQueue(IMessage message) {
//			_localQueue.DeleteMessage(message);
//		}

//		public IMessage RetrieveMessageFromQueue() {
//			var outMessage = _localQueue.RetrieveMessage();

//			return outMessage;
//		}

//		private void MessageAdded(object sender) {
//			Console.WriteLine("Commit event has fired.");
//			//var message = (TestMessage1)RetrieveMessageFromQueue();

//			//if (message is not null) {
//			//	Console.WriteLine($"Id={message.Id}");
//			//	Console.WriteLine($"Aa1={message.Aa1}");
//			//} else Console.WriteLine("Message is null.");
//		}
//	}
//}


/////////////////////////////////////////////////////////////////////////////
// using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath, ScopeContextPolicy.None);
//txnScope.BeginTransaction();
//txnScope.EnlistFile(_localQueue);

//using (_localQueue.EnterWriteScope()) {

//	var taskA = new Task(() => { _localQueue.Add(message); });
//	var taskB = new Task(() => { _localQueue.Add(message); });
//	var taskC = new Task(() => { _localQueue.Add(message); });
//	var taskD = new Task(() => { _localQueue.Add(message); });

//	// Start the task.
//	taskA.Start();
//	taskB.Start();
//	taskC.Start();
//	taskD.Start();

//	Task
//	taskA.Wait();
//	taskB.Wait();
//	taskC.Wait();
//	taskD.Wait();
//}

//txnScope.Commit();