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
	public class HeliumQueueStressTests
	{
        private IHeliumQueue _localQueue; /*Class under test*/
        private const int MessageInsertAttempts = 2;
        private const int MessageBatchSize = 3;

        private LocalQueueSettings _localQueueSettings;

		[SetUp]
        public void InitializeLocalHeliumQueue()
        {
            CreateLocalQueueForTesting();

            if (_localQueue.RequiresLoad)
                _localQueue.Load();

            _localQueue.Committed += MessageAdded;
        }

		[Test]
		public void SynchronouslyInsertMultipleMessagesIntoQueue()
		{
            _localQueue.Clear();

            var sw = new Stopwatch();
			
            sw.Start();
			var totalMessageList = InsertMessagesInQueue();
			sw.Stop();
            var duration = sw.Elapsed;

			Assert.AreEqual(totalMessageList.Count, _localQueue.Count);
            Console.WriteLine($"Time taken to insert {totalMessageList.Count} messages is: {duration.TotalSeconds} seconds");
		}

		[Test]
		public void SynchronouslyFlushQueueMultipleTimes()
		{
			_localQueue.Clear();

            var sw = new Stopwatch();
            sw.Start();
			var totalMessageList = InsertMessagesInQueue();
			Assert.AreEqual(totalMessageList.Count, _localQueue.Count);

			_localQueue.Clear();
			Assert.AreEqual(_localQueue.Count, 0);

			_localQueue.Clear();
			Assert.AreEqual(_localQueue.Count, 0);

			_localQueue.Clear();
			Assert.AreEqual(_localQueue.Count, 0);
            
            sw.Stop();
            var duration = sw.Elapsed;
            Console.WriteLine($"Time taken to insert {totalMessageList.Count} messages and flush is: {duration.TotalSeconds} seconds");
		}

		[Test]
		public void SynchronouslyDeletedMessagesFromQueue()
		{
			_localQueue.Clear();

            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
			
            sw1.Start();
			var totalMessageList = InsertMessagesInQueue();
			sw1.Stop();
            var duration1 = sw1.Elapsed;
			Assert.AreEqual(totalMessageList.Count, _localQueue.Count);
			
            sw2.Start();
			foreach (var message in totalMessageList)
				_localQueue.DeleteMessage(message);
			sw2.Stop();
            var duration2 = sw2.Elapsed;
            
            Console.WriteLine($"Time taken to insert {totalMessageList.Count} message is: {duration1.TotalSeconds} seconds");
            Console.WriteLine($"Time taken to delete {totalMessageList.Count} message is: {duration2.TotalSeconds} seconds");
			Assert.AreEqual(_localQueue.Count, 0);
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
			IList<IMessage> readFromQueueList = totalMessageList.Select(_ => RemoveMessageFromQueue()).ToList();
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
            IList<IMessage> readFromQueueList = totalMessageList.Select(_ => ReadMessageFromQueue()).ToList();
            sw2.Stop();
            var duration2 = sw2.Elapsed;

            Console.WriteLine($"Time taken to insert {totalMessageList.Count} message is: {duration1.TotalSeconds} seconds");
            Console.WriteLine($"Time taken to read {totalMessageList.Count} message is: {duration2.TotalSeconds} seconds");
            Assert.AreEqual(totalMessageList.Count, readFromQueueList.Count);
        }

		private IList<IMessage> InsertMessagesInQueue()
		{
			IList<IMessage> totalMessageList = new List<IMessage>();

			for (var i = 0; i < MessageInsertAttempts; i++)
				AddMultipleMessagesSynchronouslyToQueue().ForEach(x => totalMessageList.Add(x));
			
            return totalMessageList;
		}

        private void CreateLocalQueueForTesting()
        {
            _localQueueSettings = new LocalQueueSettings();

            if (!Directory.Exists(_localQueueSettings.TempDirPath))
                Directory.CreateDirectory(_localQueueSettings.TempDirPath);

            if (File.Exists(_localQueueSettings.Path))
                File.Delete(_localQueueSettings.Path);

            _localQueue = new LocalQueue(_localQueueSettings);
        }

        private static void MessageAdded(object sender)
        {
            Console.WriteLine("Commit event has fired.");
        }

        private IEnumerable<IMessage> AddMultipleMessagesSynchronouslyToQueue()
        {
            var messageList = GetMessageList();

            using var txnScope = new FileTransactionScope(_localQueueSettings.TempDirPath);
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

        private IList<IMessage> GetMessageList()
        {
            IList<IMessage> messageList = new List<IMessage>();

            for (var i = 0; i < MessageBatchSize; i++)
                messageList.Add(CreateMessage());

            return messageList;
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

        public IMessage RemoveMessageFromQueue()
        {
            using var txnScope = new FileTransactionScope(_localQueueSettings.TempDirPath);
            txnScope.BeginTransaction();
            txnScope.EnlistFile(_localQueue, false);

            var message = _localQueue.RemoveMessage();

            txnScope.Commit();

            return message;
        }

        public IMessage ReadMessageFromQueue()
        {
            var message = _localQueue.ReadMessage();

            return message;
        }

        //public void DeleteFileAndFolder()
        //{
        //    _localQueue?.Dispose(); // Need to dispose the queue which is using that file and folder
        //    _localQueue = null;
        //    var queuePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName);

        //    if (File.Exists(queuePath))
        //        File.Delete(queuePath);

        //    if (Directory.Exists(_queueTempDir))
        //        Directory.Delete(_queueTempDir);
        //}
    }

    public class HeliumQueueProcessor
	{
		//private readonly int _batchSize;
		//private readonly LocalQueueSettings _queueSettings;
		//private const string StrGuid = "997D1367-E7B0-46F0-B0A1-686DC0F15945";
		//private const string TempQueueName = "Temp_AB3CB3F9-3EBC-46B3-877D-14AB5A7A7FD2_1";
		//private readonly Guid _sameGuid = new Guid(StrGuid);
		//private readonly string _queueTempDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "a");
		//private IHeliumQueue _localQueue;

		//public HeliumQueueProcessor(int batchSize)
		//{
		//	_batchSize = batchSize;

		//	if (!Directory.Exists(_queueTempDir))
		//		Directory.CreateDirectory(_queueTempDir);

		//	var queuePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName);

		//	if (File.Exists(queuePath)) File.Delete(queuePath);

		//	//var queueConfig = new Sphere10.Helium.Queue.LocalQueueSettings
		//	//{
		//	//	Path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TempQueueName),
		//	//	TempDirPath = _queueTempDir,
		//	//	FileId = _sameGuid,
		//	//	TransactionalPageSizeBytes = 1 << 17, /*DefaultTransactionalPageSize = 1 << 17; => 132071 ~ 128 KB*/
		//	//	MaxStorageSizeBytes = 1 << 21, /*2097152 ~ 2MB*/
		//	//	FileMemoryCacheBytes = 1 << 20, /*1048576 ~ 1MB*/
		//	//	ClusterSize = 1 << 9, /*512 B*/
		//	//	MaxItems = 500,
		//	//	ReadOnly = false
		//	//};

		//	_queueSettings = new Sphere10.Helium.Queue.LocalQueueSettings();
		//	_localQueue = SetupHeliumQueue();

		//	if (_localQueue.RequiresLoad)
		//		_localQueue.Load();

		//	_localQueue.Committed += MessageAdded;
		//}

		//public int CountLocal()
		//{
		//	return _localQueue.Count;
		//}

		//public void ClearAll()
		//{
		//	_localQueue.Clear();
		//}

		
		
		
        

		

		//private IHeliumQueue SetupHeliumQueue()
		//{
		//	return _localQueue ??= new LocalQueue(_queueSettings);
		//}

		
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

    //public class LocalQueueSettings
    //{
    //    public Guid FileId { get; set; }
    //    public string Path { get; set; }
    //    public string TempDirPath { get; set; }
    //    public int MaxItems { get; set; }
    //    public int MaxStorageSizeBytes { get; set; }
    //    public int AllocatedMemory { get; set; }
    //    public int TransactionalPageSizeBytes { get; set; }
    //    public int ClusterSize { get; set; }
    //    public int ListingClusterCount { get; set; }
    //    public int StorageClusterCount { get; set; }
    //    public int InputQueueReadRatePerMinute { get; set; }
    //    public string ErrorQueueName { get; set; } = "ErrorQueue";
    //    public string AuditLogQueueName { get; set; } = "AuditLogQueue";
    //    public int FileMemoryCacheBytes { get; set; }
    //    public bool ReadOnly { get; set; }
    //}
}
