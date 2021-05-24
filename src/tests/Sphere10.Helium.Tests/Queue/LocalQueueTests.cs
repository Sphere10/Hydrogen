using System;
using System.IO;
using NUnit.Framework;
using Sphere10.Framework;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Tests.Queue {
	[TestFixture]
	public class LocalQueueTests {

		private LocalQueueProcessor _localQueueProcessor;
		
		[SetUp]
		public void InitializeLocalQueue() {

			_localQueueProcessor = new LocalQueueProcessor();
		}

		[Test]
		public void MultipleMessageInsertedSynchronouslyIntoLocalQueue() {
			_localQueueProcessor.ClearAll();

			for (var i = 0; i < 10; i++) 
				_localQueueProcessor.AddMultipleMessagesSynchronouslyToQueue();
		}

		[TearDown]
		public void Cleanup() {
			_localQueueProcessor.DeleteFileAndFolder();
		}
	}
	
	public class LocalQueueProcessor {
		private readonly QueueConfigDto _queueConfigDto;

		private const string StrGuid = "997D1367-E7B0-46F0-B0A1-686DC0F15945";
		private const string TempQueueName = "Temp_AB3CB3F9-3EBC-46B3-877D-14AB5A7A7FD2_1";
		private readonly Guid _sameGuid = new Guid(StrGuid);
		private readonly string _queueTempDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "a");

		private IHeliumQueue _localQueue;

		public LocalQueueProcessor() {

			if (!Directory.Exists(_queueTempDir))
				Directory.CreateDirectory(_queueTempDir);

			var queuePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName);

			if (File.Exists(queuePath)) File.Delete(queuePath);

			var queueConfig = new QueueConfigDto {
				Path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName),
				TempDirPath = _queueTempDir,
				FileId = _sameGuid,
				TransactionalPageSizeBytes = 1 << 17, /*DefaultTransactionalPageSize = 1 << 17; => 132071 ~ 128 KB*/
				MaxStorageSizeBytes = 1 << 21, /*2097152 ~ 2MB*/
				FileMemoryCacheBytes = 1 << 20, /*1048576 ~ 1MB*/
				ClusterSize = 1 << 9, /*512 B*/
				MaxItems = 500,
				ReadOnly = false
			};

			_queueConfigDto = queueConfig;

			_localQueue = SetupLocalQueue();

			if (_localQueue.RequiresLoad)
				_localQueue.Load();

			_localQueue.Committed += MessageAdded;
		}

		public int CountLocal() {
			return _localQueue.Count;
		}

		public void ClearAll() {
			_localQueue.Clear();
		}

		public void AddMultipleMessagesSynchronouslyToQueue() {

			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath, ScopeContextPolicy.None);
			txnScope.BeginTransaction();
			txnScope.EnlistFile(_localQueue);

			using (_localQueue.EnterWriteScope()) {
				_localQueue.Add(CreateMessage());
				_localQueue.Add(CreateMessage());
				_localQueue.Add(CreateMessage());
				_localQueue.Add(CreateMessage());
				_localQueue.Add(CreateMessage());
				_localQueue.Add(CreateMessage());
				_localQueue.Add(CreateMessage());
				_localQueue.Add(CreateMessage());
				_localQueue.Add(CreateMessage());
				_localQueue.Add(CreateMessage());
			}

			txnScope.Commit();
		}

		public void DeleteMessageFromQueue(IMessage message) {
			_localQueue.DeleteMessage(message);
		}

		public IMessage RetrieveMessageFromQueue() {
			if (_localQueue.Count == 0)
				throw new InvalidOperationException("CRITICAL ERROR: LocalQueue is empty and should not be empty. Message missing cannot proceed.");

			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath, ScopeContextPolicy.None);
			txnScope.BeginTransaction();
			txnScope.EnlistFile(_localQueue);

			var localQueueMessage = _localQueue[^1];

			_localQueue.RemoveAt(^1);

			txnScope.Commit();

			return localQueueMessage;
		}

		private void MessageAdded(object sender) {
			Console.WriteLine("Commit event has fired.");
		}

		private IHeliumQueue SetupLocalQueue() {
			return _localQueue ??= new LocalQueue(_queueConfigDto);
		}

		private static IMessage CreateMessage() {

			var inMessage = new TestMessage1 {
				Id = Guid.NewGuid().ToString(),
				MessageField1 = $"MessageField1 {DateTime.Now:F}",
				MessageField2 = $"MessageField2 {DateTime.Now:F}",
				MessageField3 = $"MessageField3 {DateTime.Now:F}",
				MessageField4 = $"MessageField4 {DateTime.Now:F}"
			};

			return inMessage;
		}

		public void DeleteFileAndFolder() {
			var queuePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName);

			if (File.Exists(queuePath)) File.Delete(queuePath);

			if (Directory.Exists(_queueTempDir))
				Directory.Delete(_queueTempDir);
		}
	}

	[Serializable]
	public record TestMessage1 : IMessage {
		public string Id { get; set; }
		public string MessageField1 { get; init; }
		public string MessageField2 { get; init; }
		public string MessageField3 { get; init; }
		public string MessageField4 { get; init; }
	}
}
