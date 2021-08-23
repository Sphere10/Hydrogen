using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Sphere10.Framework;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Tests.Queue
{
    [TestFixture]
    public class HeliumLocalQueueTests
    {
        private IHeliumQueue _localQueue;
        private static int _messageCommitEventCount;
        private LocalQueueSettings _localQueueSettings;
        private const int MessageBatchSize = 4;

        [SetUp]
        public void InitializeLocalHeliumQueue()
        {
            _messageCommitEventCount = 0;

            CreateLocalQueueForTesting();

            if (_localQueue.RequiresLoad)
                _localQueue.Load();

            FlushLocalQueueBeforeTesting();

            _localQueue.Committed += MessageAdded;
        }

        [Test]
        public void AddMessagesSynchronouslyToQueue()
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

            Assert.AreEqual(_messageCommitEventCount, 1);
            Assert.AreEqual(_localQueue.Count, MessageBatchSize);
        }

        [Test]
        public void ReadMessageFromQueue()
        {
            var message = _localQueue.ReadMessage();
            Assert.IsNotNull(message);
        }

        private static IMessage CreateMessage(string messageId)
        {
            var message = new TestMessage10
            {
                Id = messageId,
                MessageField1 = 1,
                MessageField2 = 2,
                MessageField3 = "TestMessage",
                MessageField4 = DateTime.Now
            };

            return message;
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

        private void FlushLocalQueueBeforeTesting()
        {
            var messageCount = _localQueue.Count;
            if (messageCount > 0) _localQueue.Clear();
        }

        private static void MessageAdded(object sender)
        {
            Console.WriteLine("Message added HOORAY!");
            _messageCommitEventCount++;
        }

        private IList<IMessage> GetMessageList()
        {
            IList<IMessage> messageList = new List<IMessage>();

            for (var i = 0; i < MessageBatchSize; i++)
                messageList.Add(CreateMessage($"TestMessageA{i}"));

            return messageList;
        }
    }

    [Serializable]
    public record TestMessage10 : IMessage
    {
        public string Id { get; set; }
        public int MessageField1 { get; init; }
        public int MessageField2 { get; init; }
        public string MessageField3 { get; init; }
        public DateTime MessageField4 { get; init; }
    }
}