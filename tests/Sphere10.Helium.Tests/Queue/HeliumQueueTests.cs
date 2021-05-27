using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Sphere10.Framework;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Tests.Queue
{
	[TestFixture]
	public class HeliumQueueTests
	{
		private HeliumQueueProcessor _localQueueProcessor;
		private const int MessageInsertAttempts = 10;
		private const int MessageBatchSize = 40;

		[SetUp]
		public void InitializeHeliumQueue()
		{
			_localQueueProcessor = new HeliumQueueProcessor(MessageBatchSize);
		}

		[Test]
		public void SynchronouslyInsertMultipleMessagesIntoQueue()
		{
			_localQueueProcessor.ClearAll();

            var sw = new Stopwatch();
			
            sw.Start();
			var totalMessageList = InsertMessagesInQueue();
			sw.Stop();
            var duration = sw.Elapsed;

			Assert.AreEqual(totalMessageList.Count, _localQueueProcessor.CountLocal());
            Console.WriteLine($"Time taken to insert {totalMessageList.Count} messages is: {duration.TotalSeconds} seconds");
		}

		[Test]
		public void SynchronouslyFlushQueueMultipleTimes()
		{
			_localQueueProcessor.ClearAll();

            var sw = new Stopwatch();
            sw.Start();
			var totalMessageList = InsertMessagesInQueue();
			Assert.AreEqual(totalMessageList.Count, _localQueueProcessor.CountLocal());

			_localQueueProcessor.ClearAll();
			Assert.AreEqual(_localQueueProcessor.CountLocal(), 0);

			_localQueueProcessor.ClearAll();
			Assert.AreEqual(_localQueueProcessor.CountLocal(), 0);

			_localQueueProcessor.ClearAll();
			Assert.AreEqual(_localQueueProcessor.CountLocal(), 0);
            
            sw.Stop();
            var duration = sw.Elapsed;
            Console.WriteLine($"Time taken to insert {totalMessageList.Count} messages and flush is: {duration.TotalSeconds} seconds");
		}

		[Test]
		public void SynchronouslyDeletedMessagesFromQueue()
		{
			_localQueueProcessor.ClearAll();

            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
			
            sw1.Start();
			var totalMessageList = InsertMessagesInQueue();
			sw1.Stop();
            var duration1 = sw1.Elapsed;
			Assert.AreEqual(totalMessageList.Count, _localQueueProcessor.CountLocal());
			
            sw2.Start();
			foreach (var message in totalMessageList)
				_localQueueProcessor.DeleteMessageFromQueue(message);
			sw2.Stop();
            var duration2 = sw2.Elapsed;
            
            Console.WriteLine($"Time taken to insert {totalMessageList.Count} message is: {duration1.TotalSeconds} seconds");
            Console.WriteLine($"Time taken to delete {totalMessageList.Count} message is: {duration2.TotalSeconds} seconds");
			Assert.AreEqual(_localQueueProcessor.CountLocal(), 0);
		}

		[Test]
		public void SynchronouslyRemoveFirstMessageFromQueue()
		{
            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
            
            sw1.Start();
			var totalMessageList = InsertMessagesInQueue();
			sw1.Stop();
            var duration1 = sw1.Elapsed;

            sw2.Start();
			IList<IMessage> readFromQueueList = totalMessageList.Select(_ => _localQueueProcessor.RemoveMessageFromQueue()).ToList();
            sw2.Stop();
			var duration2 = sw2.Elapsed;

            Console.WriteLine($"Time taken to insert {totalMessageList.Count} message is: {duration1.TotalSeconds} seconds");
            Console.WriteLine($"Time taken to remove {totalMessageList.Count} message is: {duration2.TotalSeconds} seconds");
			Assert.AreEqual(totalMessageList.Count, readFromQueueList.Count);
		}

        [Test]
        public void SynchronouslyReadFirstMessageFromQueue()
        {
            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();

            sw1.Start();
            var totalMessageList = InsertMessagesInQueue();
            sw1.Stop();
            var duration1 = sw1.Elapsed;

            sw2.Start();
            IList<IMessage> readFromQueueList = totalMessageList.Select(_ => _localQueueProcessor.ReadMessageFromQueue()).ToList();
            sw2.Stop();
            var duration2 = sw2.Elapsed;

            Console.WriteLine($"Time taken to insert {totalMessageList.Count} message is: {duration1.TotalSeconds} seconds");
            Console.WriteLine($"Time taken to read {totalMessageList.Count} message is: {duration2.TotalSeconds} seconds");
            Assert.AreEqual(totalMessageList.Count, readFromQueueList.Count);
        }

		[TearDown]
		public void Cleanup()
		{
			_localQueueProcessor.DeleteFileAndFolder();
		}

		private IList<IMessage> InsertMessagesInQueue()
		{
			IList<IMessage> totalMessageList = new List<IMessage>();

			for (var i = 0; i < MessageInsertAttempts; i++)
				_localQueueProcessor.AddMultipleMessagesSynchronouslyToQueue().ForEach(x => totalMessageList.Add(x));
			
            return totalMessageList;
		}
	}

	public class HeliumQueueProcessor
	{
		private readonly int _batchSize;
		private readonly QueueConfigDto _queueConfigDto;
		private const string StrGuid = "997D1367-E7B0-46F0-B0A1-686DC0F15945";
		private const string TempQueueName = "Temp_AB3CB3F9-3EBC-46B3-877D-14AB5A7A7FD2_1";
		private readonly Guid _sameGuid = new Guid(StrGuid);
		private readonly string _queueTempDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "a");
		private IHeliumQueue _localQueue;

		public HeliumQueueProcessor(int batchSize)
		{
			_batchSize = batchSize;

			if (!Directory.Exists(_queueTempDir))
				Directory.CreateDirectory(_queueTempDir);

			var queuePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName);

			if (File.Exists(queuePath)) File.Delete(queuePath);

			var queueConfig = new QueueConfigDto
			{
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
			_localQueue = SetupHeliumQueue();

			if (_localQueue.RequiresLoad)
				_localQueue.Load();

			_localQueue.Committed += MessageAdded;
		}

		public int CountLocal()
		{
			return _localQueue.Count;
		}

		public void ClearAll()
		{
			_localQueue.Clear();
		}

		public IList<IMessage> AddMultipleMessagesSynchronouslyToQueue()
		{
			var messageList = GetMessageList();

			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath);
			txnScope.BeginTransaction();
			txnScope.EnlistFile(_localQueue, false);

			using (_localQueue.EnterWriteScope())
			{
				foreach (var message in messageList)
					_localQueue.AddMessage(message);
			}

			txnScope.Commit();

			return messageList;
		}
		
		public IMessage RemoveMessageFromQueue()
		{
			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath);
			txnScope.BeginTransaction();
			txnScope.EnlistFile(_localQueue, false);

            var message = _localQueue.RemoveMessage();

			txnScope.Commit();

			return message;
		}

        public IMessage ReadMessageFromQueue()
        {
            var message = _localQueue.RemoveMessage();

            return message;
        }

		public void DeleteFileAndFolder()
        {
            _localQueue?.Dispose(); // Need to dispose the queue which is using that file and folder
            _localQueue = null;
            var queuePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName);

            if (File.Exists(queuePath)) 
                File.Delete(queuePath);

            if (Directory.Exists(_queueTempDir))
                Directory.Delete(_queueTempDir);
        }

		public void DeleteMessageFromQueue(IMessage message)
        {
            _localQueue.DeleteMessage(message);
        }

        private IList<IMessage> GetMessageList()
        {
            IList<IMessage> messageList = new List<IMessage>();

            for (var i = 0; i < _batchSize; i++)
                messageList.Add(CreateMessage());

            return messageList;
        }

		private static void MessageAdded(object sender)
		{
			Console.WriteLine("Commit event has fired.");
		}

		private IHeliumQueue SetupHeliumQueue()
		{
			return _localQueue ??= new LocalQueue(_queueConfigDto);
		}

		private static IMessage CreateMessage()
		{
			var inMessage = new TestMessage1
			{
				Id = Guid.NewGuid().ToString(),
				MessageField1 = $"MessageField1 {DateTime.Now:F}",
				MessageField2 = $"MessageField2 {DateTime.Now:F}",
				MessageField3 = $"MessageField3 {DateTime.Now:F}",
				MessageField4 = $"MessageField4 {DateTime.Now:F}"
			};

			return inMessage;
		}
	}

	[Serializable]
	public record TestMessage1 : IMessage
	{
		public string Id { get; set; }
		public string MessageField1 { get; init; }
		public string MessageField2 { get; init; }
		public string MessageField3 { get; init; }
		public string MessageField4 { get; init; }
	}
}
